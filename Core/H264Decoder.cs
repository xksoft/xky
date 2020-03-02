using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using FFmpeg.AutoGen;

namespace Xky.Core
{
    public unsafe class H264Decoder
    {
        private const string LdLibraryPath = "LD_LIBRARY_PATH";
        private readonly AVCodecContext* _pCodecCtx;
        private readonly AVCodecParserContext* _pCodecParserCtx;
        private readonly AVFrame* _pFrame;
        internal bool Firstpacket = true;


        public H264Decoder()
        {
            try
            {
                RegisterFFmpegBinaries();
                var pCodec = ffmpeg.avcodec_find_decoder(AVCodecID.AV_CODEC_ID_H264);

                _pCodecCtx = ffmpeg.avcodec_alloc_context3(pCodec);
                _pCodecCtx->qcompress = 1F;
                _pCodecCtx->frame_number = 1;
                _pCodecCtx->codec_type = AVMediaType.AVMEDIA_TYPE_VIDEO;
                _pCodecParserCtx = ffmpeg.av_parser_init((int) AVCodecID.AV_CODEC_ID_H264);
                if (null == _pCodecParserCtx)
                    throw new Exception("_pCodecParserCtx is null");

                if (pCodec->capabilities > 0) _pCodecCtx->flags |= ffmpeg.AV_CODEC_FLAG_TRUNCATED;


                var ret = ffmpeg.avcodec_open2(_pCodecCtx, pCodec, null);
                if (ret < 0)
                    throw new Exception("ret is null");

                _pFrame = ffmpeg.av_frame_alloc();
            }
            catch
            {
                throw new Exception("dll is null");
            }
        }


        internal void Decode(byte[] h264Data)
        {
            //加入速率计数器
            Client.BitAverageNumber.Push(h264Data.Length);
            var curSize = h264Data.Length;
            var curPtr = (byte*) ffmpeg.av_malloc((ulong) curSize);
            try
            {
                for (var i = 0; i < curSize; i++) curPtr[i] = h264Data[i];

                while (curSize > 0)
                {
                    AVPacket packet;
                    if (Firstpacket)
                    {
                        packet.size = curSize;
                        packet.data = curPtr;
                        ffmpeg.av_init_packet(&packet);
                        Firstpacket = false;
                    }


                    var len = ffmpeg.av_parser_parse2(_pCodecParserCtx, _pCodecCtx,
                        &packet.data, &packet.size, curPtr, curSize,
                        ffmpeg.AV_NOPTS_VALUE, ffmpeg.AV_NOPTS_VALUE, ffmpeg.AV_NOPTS_VALUE);

                    curSize -= len;
                    if (packet.size == 0)
                        continue;

                    ffmpeg.avcodec_send_packet(_pCodecCtx, &packet);

                    var ret = ffmpeg.avcodec_receive_frame(_pCodecCtx, _pFrame);

                    while (ret >= 0)
                    {
                        using (var vfc = new VideoFrameConverter(new Size(_pFrame->width, _pFrame->height),
                            _pCodecCtx->pix_fmt, new Size(_pFrame->width, _pFrame->height),
                            AVPixelFormat.AV_PIX_FMT_BGR24))
                        {
                            var convertedFrame = vfc.Convert(*_pFrame);

                            //调用事件
                            OnDecodeBitmapSource?.Invoke(this, convertedFrame.width, convertedFrame.height,
                                convertedFrame.linesize[0], (IntPtr) convertedFrame.data[0]);

                            //释放内存吗？
                            ffmpeg.av_frame_unref(&convertedFrame);
                        }


                        ret = ffmpeg.avcodec_receive_frame(_pCodecCtx, _pFrame);
                    }

                    //释放内存吗？
                    ffmpeg.av_packet_unref(&packet);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                ffmpeg.av_free(curPtr);
            }
        }


        internal static void RegisterFFmpegBinaries()
        {
            var current = Environment.CurrentDirectory;
            var probe = Path.Combine(Environment.Is64BitProcess ? "x64" : "x86");
            while (current != null)
            {
                var ffmpegDirectory = Path.Combine(current, probe);
                if (Directory.Exists(ffmpegDirectory))
                {
                    Console.WriteLine($"dll binaries found in: {ffmpegDirectory}");
                    RegisterLibrariesSearchPath(ffmpegDirectory);
                    return;
                }

                current = Directory.GetParent(current)?.FullName;
            }
        }

        private static void RegisterLibrariesSearchPath(string path)
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                    SetDllDirectory(path);
                    break;
                case PlatformID.Unix:
                case PlatformID.MacOSX:
                    var currentValue = Environment.GetEnvironmentVariable(LdLibraryPath);
                    if (string.IsNullOrWhiteSpace(currentValue) == false && currentValue.Contains(path) == false)
                    {
                        var newValue = currentValue + Path.PathSeparator + path;
                        Environment.SetEnvironmentVariable(LdLibraryPath, newValue);
                    }

                    break;
            }
        }

        [DllImport("kernel32", SetLastError = true)]
        private static extern bool SetDllDirectory(string lpPathName);

        internal event DecodeBitmap OnDecodeBitmapSource;

        internal delegate void DecodeBitmap(object sender, int width, int height, int stride, IntPtr intprt);

        private sealed class VideoFrameConverter : IDisposable
        {
            private readonly IntPtr _convertedFrameBufferPtr;
            private readonly Size _destinationSize;
            private readonly byte_ptrArray4 _dstData;
            private readonly int_array4 _dstLinesize;
            private readonly SwsContext* _pConvertContext;

            internal VideoFrameConverter(Size sourceSize, AVPixelFormat sourcePixelFormat,
                Size destinationSize, AVPixelFormat destinationPixelFormat)
            {
                _destinationSize = destinationSize;

                _pConvertContext = ffmpeg.sws_getContext(sourceSize.Width, sourceSize.Height, sourcePixelFormat,
                    destinationSize.Width,
                    destinationSize.Height, destinationPixelFormat,
                    ffmpeg.SWS_FAST_BILINEAR, null, null, null);
                if (_pConvertContext == null)
                    throw new ApplicationException("Could not initialize the conversion context.");

                var convertedFrameBufferSize = ffmpeg.av_image_get_buffer_size(destinationPixelFormat,
                    destinationSize.Width, destinationSize.Height, 1);
                _convertedFrameBufferPtr = Marshal.AllocHGlobal(convertedFrameBufferSize);
                _dstData = new byte_ptrArray4();
                _dstLinesize = new int_array4();

                ffmpeg.av_image_fill_arrays(ref _dstData, ref _dstLinesize, (byte*) _convertedFrameBufferPtr,
                    destinationPixelFormat, destinationSize.Width, destinationSize.Height, 1);
            }

            public void Dispose()
            {
                Marshal.FreeHGlobal(_convertedFrameBufferPtr);
                ffmpeg.sws_freeContext(_pConvertContext);
            }

            internal AVFrame Convert(AVFrame sourceFrame)
            {
                ffmpeg.sws_scale(_pConvertContext, sourceFrame.data, sourceFrame.linesize, 0, sourceFrame.height,
                    _dstData, _dstLinesize);

                var data = new byte_ptrArray8();
                data.UpdateFrom(_dstData);
                var linesize = new int_array8();
                linesize.UpdateFrom(_dstLinesize);

                return new AVFrame
                {
                    data = data,
                    linesize = linesize,
                    width = _destinationSize.Width,
                    height = _destinationSize.Height
                };
            }
        }
    }
}
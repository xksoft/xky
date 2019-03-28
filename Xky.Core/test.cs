using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using FFmpeg.AutoGen;

namespace Xky.Core
{
    public unsafe class H264Parser
    {

        /// <summary>
        /// 解码H264网络流
        /// </summary>
        /// <param name="socket">Socket对象</param>
        public unsafe void Parser(Socket socket)
        {
            AVCodecContext* pCodecCtx = null;
            AVCodecParserContext* pCodecParserCtx = null;
            AVCodec* pCodec = null;
            AVFrame* pFrame = null;             //yuv 
            AVPacket packet;                    //h264 
            AVPicture picture;                  //储存rgb格式图片 
            SwsContext* pSwsCtx = null;
            AVCodecID codec_id = AVCodecID.AV_CODEC_ID_H264;

            int ret;

            //FFmpeg可执行二进制命令工具查找
            //FFmpegBinariesHelper.RegisterFFmpegBinaries();

            //ffmpeg.av_register_all();
            //ffmpeg.avcodec_register_all();

            /* 初始化AVCodec */
            pCodec = ffmpeg.avcodec_find_decoder(codec_id);

            /* 初始化AVCodecContext,只是分配，还没打开 */
            pCodecCtx = ffmpeg.avcodec_alloc_context3(pCodec);

            /* 初始化AVCodecParserContext */
            pCodecParserCtx = ffmpeg.av_parser_init((int)AVCodecID.AV_CODEC_ID_H264);
            if (null == pCodecParserCtx)
            {
                return;//终止执行
            }

//            /* we do not send complete frames,什么意思？ */
//            if (pCodec->capabilities > 0 && ffmpeg.CODEC_CAP_TRUNCATED > 0)
//                pCodecCtx->flags |= ffmpeg.CODEC_FLAG_TRUNCATED;

            /* 打开解码器 */
            ret = ffmpeg.avcodec_open2(pCodecCtx, pCodec, null);
            if (ret < 0)
            {
                return;//终止执行
            }


            pFrame = ffmpeg.av_frame_alloc();
            ffmpeg.av_init_packet(&packet);
            packet.size = 0;
            packet.data = null;

            const int in_buffer_size = 4096;
            //uint in_buffer[in_buffer_size + FF_INPUT_BUFFER_PADDING_SIZE] = { 0 };
            byte[] in_buffer = new byte[in_buffer_size];
            byte* cur_ptr;
            int cur_size;
            int got;
            bool is_first_time = true;

            while (true)
            {
                // Socket通信实例接收信息
                //cur_size = 0;//recv(m_socket, (char*)in_buffer, in_buffer_size, 0);
                cur_size = socket.Receive(in_buffer, in_buffer_size, SocketFlags.None);
                Console.WriteLine("H264Parser Socket Receive:  data byte string={0}", BitConverter.ToString(in_buffer));
                if (cur_size == 0)
                    break;

                //cur_ptr = in_buffer;//指针转换问题
                cur_ptr = (byte*)ffmpeg.av_malloc(in_buffer_size);
                while (cur_size > 0)
                {
                    /* 返回解析了的字节数 */
                    int len = ffmpeg.av_parser_parse2(pCodecParserCtx, pCodecCtx,
                        &packet.data, &packet.size, (byte*)cur_ptr, cur_size,
                        ffmpeg.AV_NOPTS_VALUE, ffmpeg.AV_NOPTS_VALUE, ffmpeg.AV_NOPTS_VALUE);
                    cur_ptr += len;
                    cur_size -= len;
                    if (packet.size == 0)
                        continue;

                    //switch (pCodecParserCtx->pict_type) 
                    //{ 
                    //  case AV_PICTURE_TYPE_I: printf("Type: I\t"); break; 
                    //  case AV_PICTURE_TYPE_P: printf("Type: P\t"); break; 
                    //  case AV_PICTURE_TYPE_B: printf("Type: B\t"); break; 
                    //  default: printf("Type: Other\t"); break; 
                    //} 
                    //printf("Output Number:%4d\t", pCodecParserCtx->output_picture_number); 
                    //printf("Offset:%8ld\n", pCodecParserCtx->cur_offset); 
                    ret = ffmpeg.avcodec_decode_video2(pCodecCtx, pFrame, &got, &packet);
                    if (ret < 0)
                    {
                        return;//终止执行
                    }

                    if (got > 0)
                    {
                        if (is_first_time)  //分配格式转换存储空间 
                        {
                            // C AV_PIX_FMT_RGB32 统一改为 AVPixelFormat.AV_PIX_FMT_RGB24
                            pSwsCtx = ffmpeg.sws_getContext(pCodecCtx->width, pCodecCtx->height, pCodecCtx->pix_fmt,
                                pCodecCtx->width, pCodecCtx->height, AVPixelFormat.AV_PIX_FMT_RGB24, ffmpeg.SWS_BICUBIC, null, null, null);

                            ffmpeg.avpicture_alloc(&picture, AVPixelFormat.AV_PIX_FMT_RGB24, pCodecCtx->width, pCodecCtx->height);

                            is_first_time = false;
                        }
                        /* YUV转RGB */
                        ffmpeg.sws_scale(pSwsCtx, pFrame->data, pFrame->linesize,
                            0, pCodecCtx->height,
                            picture.data, picture.linesize);

                        //QImage img(picture.data[0], pCodecCtx->width, pCodecCtx->height, QImage::Format_RGB32);

                        //emit this-> signal_receive_one_image(img);

                        //填充视频帧
                        //ffmpeg.avpicture_fill((AVPicture*)pFrame, cur_ptr, AVPixelFormat.AV_PIX_FMT_RGB24, pCodecCtx->width, pCodecCtx->height);

                        #region 构造图片
                        var dstData = new byte_ptrArray4();// 声明形参
                        var dstLinesize = new int_array4();// 声明形参
                        // 目标媒体格式需要的字节长度
                        var convertedFrameBufferSize = ffmpeg.av_image_get_buffer_size(AVPixelFormat.AV_PIX_FMT_RGB24, pCodecCtx->width, pCodecCtx->height, 1);
                        // 分配目标媒体格式内存使用
                        var convertedFrameBufferPtr = Marshal.AllocHGlobal(convertedFrameBufferSize);
                        // 设置图像填充参数
                        ffmpeg.av_image_fill_arrays(ref dstData, ref dstLinesize, (byte*)convertedFrameBufferPtr, AVPixelFormat.AV_PIX_FMT_RGB24, pCodecCtx->width, pCodecCtx->height, 1);

                        // 封装Bitmap图片
                        var bitmap = new Bitmap(pCodecCtx->width, pCodecCtx->height, dstLinesize[0], PixelFormat.Format24bppRgb, convertedFrameBufferPtr);

                        #endregion
                    }
                }
            }


            ffmpeg.av_free_packet(&packet);
            ffmpeg.av_frame_free(&pFrame);
            ffmpeg.avpicture_free(&picture);
            ffmpeg.sws_freeContext(pSwsCtx);
            ffmpeg.avcodec_free_context(&pCodecCtx);
            ffmpeg.av_parser_close(pCodecParserCtx);
        }

    }
}
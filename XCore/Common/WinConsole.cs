using System;
using System.IO;
using System.Runtime.InteropServices;

namespace XCore.Common
{
    /// <summary>
    /// 控制台管理类
    /// </summary>
    public static class WinConsole
    {
        [DllImport("kernel32")]
        static extern bool AllocConsole();

        [DllImport("kernel32")]
        private static extern bool FreeConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetStdHandle(int nStdHandle, IntPtr hHandle);

        private const int StdOutputHandle = -11;
        private static IntPtr _hRealOut;

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CreateFile([MarshalAs(UnmanagedType.LPTStr)] string filename,
            [MarshalAs(UnmanagedType.U4)] uint access,
            [MarshalAs(UnmanagedType.U4)] FileShare share,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] FileAttributes flagsAndAttributes,
            IntPtr templateFile);

        [DllImport("kernel32", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr handle);

        private const uint GenericWrite = 0x40000000;
        private const uint GenericRead = 0x80000000;


        private static void OverrideRedirection()
        {
            var hOut = GetStdHandle(StdOutputHandle);
            _hRealOut = CreateFile("CONOUT$", GenericRead | GenericWrite, FileShare.Write, IntPtr.Zero,
                FileMode.OpenOrCreate, 0, IntPtr.Zero);
            if (_hRealOut != hOut)
            {
                SetStdHandle(StdOutputHandle, _hRealOut);
                Console.SetOut(
                    new StreamWriter(Console.OpenStandardOutput(), Console.OutputEncoding) {AutoFlush = true});
            }
        }

        /// <summary>
        /// 显示控制台
        /// </summary>
        public static void ShowConsole()
        {
            //释放句柄，不然无法关闭控制台
            if (_hRealOut != (IntPtr) 0)
            {
                CloseHandle(_hRealOut);
                FreeConsole();
            }

            AllocConsole();
            OverrideRedirection();
        }

        /// <summary>
        /// 关闭控制台
        /// </summary>
        public static void CloseConsole()
        {
            //释放句柄，不然无法关闭控制台
            CloseHandle(_hRealOut);
            _hRealOut = (IntPtr) 0;
            FreeConsole();
        }
    }
}
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Xky.Platform
{
    public static class WinConsole
    {
        [DllImport("kernel32")]
        static extern bool AllocConsole();

        [DllImport("kernel32")]
        private static extern bool FreeConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetStdHandle(int nStdHandle, IntPtr hHandle);

        public const int StdOutputHandle = -11;

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CreateFile([MarshalAs(UnmanagedType.LPTStr)] string filename,
            [MarshalAs(UnmanagedType.U4)] uint access,
            [MarshalAs(UnmanagedType.U4)] FileShare share,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] FileAttributes flagsAndAttributes,
            IntPtr templateFile);

        public const uint GenericWrite = 0x40000000;
        public const uint GenericRead = 0x80000000;

        private static void OverrideRedirection()
        {
            var hOut = GetStdHandle(StdOutputHandle);
            var hRealOut = CreateFile("CONOUT$", GenericRead | GenericWrite, FileShare.Write, IntPtr.Zero,
                FileMode.OpenOrCreate, 0, IntPtr.Zero);
            if (hRealOut != hOut)
            {
                SetStdHandle(StdOutputHandle, hRealOut);
                Console.SetOut(
                    new StreamWriter(Console.OpenStandardOutput(), Console.OutputEncoding) {AutoFlush = true});
            }
        }

        /// <summary>
        /// 显示控制台
        /// </summary>
        public static void ShowConsole()
        {
            FreeConsole();
            AllocConsole();
            OverrideRedirection();
        }
        /// <summary>
        /// 关闭控制台
        /// </summary>
        public static void CloseConsole()
        {
            FreeConsole();
        }


    }
}
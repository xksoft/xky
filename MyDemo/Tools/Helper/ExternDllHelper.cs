using System.Runtime.InteropServices;

namespace MyDemo.Tools.Helper
{
    internal class ExternDllHelper
    {
        private const string Winmm = "winmm.dll";

        [DllImport(Winmm, EntryPoint = "mciSendString", CharSet = CharSet.Auto)]
        public static extern int MciSendString(string lpstrCommand, string lpstrReturnString, int uReturnLength, int hwndCallback);
    }
}
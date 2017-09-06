using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Prime.Utility
{
    /// <summary>
    /// https://stackoverflow.com/a/29274238/1318333
    /// </summary>
    public static class ProcessExiter
    {
        public static bool Exit(Process process)
        {
            if (!AttachConsole((uint) process.Id))
                return false;

            SetConsoleCtrlHandler(null, true);
            try
            {
                if (!GenerateConsoleCtrlEvent(CTRL_C_EVENT, 0))
                    return false;
                process.WaitForExit();
            }
            finally
            {
                FreeConsole();
                SetConsoleCtrlHandler(null, false);
            }
            return true;
        }

        internal const int CTRL_C_EVENT = 0;
        [DllImport("kernel32.dll")]
        internal static extern bool GenerateConsoleCtrlEvent(uint dwCtrlEvent, uint dwProcessGroupId);
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool AttachConsole(uint dwProcessId);
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        internal static extern bool FreeConsole();
        [DllImport("kernel32.dll")]
        static extern bool SetConsoleCtrlHandler(ConsoleCtrlDelegate HandlerRoutine, bool Add);
        // Delegate type to be used as the Handler Routine for SCCH
        delegate Boolean ConsoleCtrlDelegate(uint CtrlType);
    }
}
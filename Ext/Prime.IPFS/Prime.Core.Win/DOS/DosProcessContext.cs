using System;
using System.Diagnostics;

namespace Prime.IPFS
{
    public class DosProcessContext : ProcessContext
    {
        public DosProcessContext(string command) : base(command) { }

        public DosProcessContext(string command, Func<string, DosCancellation> checkLog, Func<string, DosCancellation> checkError, Action<Process> onProcessCreated = null, Action onProcessEnded = null) : base(command)
        {
            CheckLog = checkLog;
            CheckError = checkError;
            OnProcessCreated = onProcessCreated;
            OnProcessEnded = onProcessEnded;
        }

        public Func<string, DosCancellation> CheckLog { get; set; }
        public Func<string, DosCancellation> CheckError { get; set; }
        public Action<Process> OnProcessCreated { get; set; }
        public Action OnProcessEnded { get; set; }
    }
}
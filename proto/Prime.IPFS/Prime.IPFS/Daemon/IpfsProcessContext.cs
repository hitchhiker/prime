using System;
using System.Diagnostics;

namespace Prime.Radiant
{
    public class IpfsProcessContext
    {
        public IpfsProcessContext(string command)
        {
            Command = command;
        }

        public IpfsProcessContext(string command, Func<string, DosCancellation> checkLog, Func<string, DosCancellation> checkError, Action<Process> onProcessCreated = null, Action onProcessEnded = null) : this(command)
        {
            CheckLog = checkLog;
            CheckError = checkError;
            OnProcessCreated = onProcessCreated;
            OnProcessEnded = onProcessEnded;
        }

        public readonly string Command;
        public Func<string, DosCancellation> CheckLog { get; set; }
        public Func<string, DosCancellation> CheckError { get; set; }
        public Action<Process> OnProcessCreated { get; set; }
        public Action OnProcessEnded { get; set; }
    }
}
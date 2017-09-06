using System;
using System.Diagnostics;

namespace Prime.Radiant
{
    public class IpfsDeployStatus
    {
        public bool IsCompleted { get; set; }

        public Action OnClose { get; set; }

        public bool RequiresRespark { get; private set; }

        public bool IsDnsFailed { get; private set; }

        public bool IsFailed => !IsCompleted;

        public static IpfsDeployStatus Failed => new IpfsDeployStatus();

        public static IpfsDeployStatus Completed => new IpfsDeployStatus() {IsCompleted = true};

        public static IpfsDeployStatus DoRespark => new IpfsDeployStatus() { RequiresRespark = true};

        public static IpfsDeployStatus DnsFailed => new IpfsDeployStatus() { IsDnsFailed = true};
    }
}
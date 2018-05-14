using System;
using Prime.Core;

namespace Prime.IPFS
{
    public static class DaemonStateExtensionMethods
    {
        public static ServiceStatus GetServiceStatus(this DaemonState daemonState)
        {
            switch (daemonState)
            {
                case DaemonState.Running:
                    return ServiceStatus.Running;
                case DaemonState.Starting:
                    return ServiceStatus.Starting;
                case DaemonState.Stopped:
                    return ServiceStatus.Stopped;
                case DaemonState.Stopping:
                    return ServiceStatus.Stopping;
                case DaemonState.System:
                    return ServiceStatus.Running;
                default:
                    throw new ArgumentOutOfRangeException(nameof(daemonState), daemonState, null);
            }
        }
    }
}
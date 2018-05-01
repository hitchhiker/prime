using System;
using System.Threading;

namespace Prime.IPFS
{
    public static class IpfsExtensionMethods
    {
        public static void WaitTillRunning(this IpfsDaemonBase daemon, Action onRunning)
        {
            daemon.Start();

            enjoyGotoPurists:

            if (daemon.State() == IpFsDaemonState.Running || daemon.State() == IpFsDaemonState.System)
            {
                onRunning.Invoke();
                return;
            }

            Thread.Sleep(1);

            goto enjoyGotoPurists;
        }

        public static T WaitTillRunning<T>(this IpfsDaemonBase daemon, Func<T> onRunning)
        {
            daemon.Start();

            enjoyGotoPurists:

            if (daemon.State() == IpFsDaemonState.Running || daemon.State() == IpFsDaemonState.System)
                return onRunning.Invoke();
            
            Thread.Sleep(1);

            goto enjoyGotoPurists;
        }
    }
}
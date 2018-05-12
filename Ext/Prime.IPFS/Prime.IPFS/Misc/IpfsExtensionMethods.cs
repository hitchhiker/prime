using System;
using System.Threading;

namespace Prime.IPFS
{
    public static class IpfsExtensionMethods
    {
        public static void WaitTillRunning(this IpfsInstance instance, Action onRunning)
        {
            instance.Daemon.WaitTillRunning(onRunning);
        }

        public static T WaitTillRunning<T>(this IpfsInstance instance, Func<T> onRunning)
        {
            return instance.Daemon.WaitTillRunning(onRunning);
        }

        public static void WaitTillRunning(this IpfsDaemonBase daemon, Action onRunning)
        {
            daemon.Start();

            enjoyGotoPurists:

            if (daemon.State() == DaemonState.Running || daemon.State() == DaemonState.System)
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

            if (daemon.State() == DaemonState.Running || daemon.State() == DaemonState.System)
                return onRunning.Invoke();
            
            Thread.Sleep(1);

            goto enjoyGotoPurists;
        }
    }
}
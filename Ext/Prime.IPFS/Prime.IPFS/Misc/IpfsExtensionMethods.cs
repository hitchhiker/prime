using System;
using System.Threading;

namespace Prime.IPFS
{
    public static class IpfsExtensionMethods
    {
        public static bool IsRunning(this IpfsDaemonBase daemon)
        {
            return daemon.State() == DaemonState.Running || daemon.State() == DaemonState.System;
        }

        public static bool IsStoppedOrSystem(this IpfsDaemonBase daemon)
        {
            return daemon.State() == DaemonState.Stopped || daemon.State() == DaemonState.System;
        }

        public static void WaitTillRunning(this IpfsInstance instance, Action onRunning, TimeSpan? timeout = null)
        {
            instance.Daemon.WaitTillRunning(onRunning, timeout);
        }

        public static T WaitTillRunning<T>(this IpfsInstance instance, Func<T> onRunning, TimeSpan? timeout = null)
        {
            return instance.Daemon.WaitTillRunning(onRunning, timeout);
        }

        public static void WaitTillRunning(this IpfsDaemonBase daemon, Action onRunning, TimeSpan? timeout = null)
        {
            var expire = DateTime.UtcNow.Add(timeout ?? TimeSpan.FromSeconds(30));

            do
            {
                if (daemon.IsRunning())
                {
                    onRunning.Invoke();
                    return;
                }
                Thread.Sleep(1);
            }
            while (DateTime.UtcNow < expire);
        }

        public static T WaitTillRunning<T>(this IpfsDaemonBase daemon, Func<T> onRunning, TimeSpan? timeout = null)
        {
            var expire = DateTime.UtcNow.Add(timeout ?? TimeSpan.FromSeconds(30));

            do
            {
                if (daemon.IsRunning())
                    return onRunning.Invoke();
                Thread.Sleep(1);
            }
            while (DateTime.UtcNow < expire);

            return default(T);
        }
    }
}
using Prime.Core;
using Prime.IPFS.Win64;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Prime.IPFS
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = new ConsoleLogger();

            var dir = new DirectoryInfo("c://tmp//ipfs-ext");

            var pc = PrimeContext.ForDevelopmentServer();

            var ctx = new IpfsInstanceContext(pc, new IpfsPlatformWin64(new IpfsWin64Extension()))
            {
                L = logger
            };

            var ipfs = new IpfsInstance(ctx);

            ipfs.Daemon.Start();

            /* cleanup */
            bool ConsoleEventCallback(int eventType)
            {
                if (eventType == 2)
                    Stop(ipfs);
                return false;
            }

            handler = new ConsoleEventDelegate(ConsoleEventCallback);
            SetConsoleCtrlHandler(handler, true);
            /* cleanup */

            Console.ReadLine();
            Stop(ipfs);
        }

        private static void Stop(IpfsInstance ipfs)
        {
            ipfs.Daemon.Stop();
            do
            {
                Thread.Sleep(1);
            } while (ipfs.Daemon.State() != DaemonState.Stopped && ipfs.Daemon.State() != DaemonState.System);
        }

        static ConsoleEventDelegate handler;   // Keeps it from getting garbage collected
        // Pinvoke
        private delegate bool ConsoleEventDelegate(int eventType);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);
    }
}

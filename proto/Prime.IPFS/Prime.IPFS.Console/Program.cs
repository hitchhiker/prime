using Prime.Core;
using Prime.IPFS.Win64;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.IPFS
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = new ConsoleLogger();

            var dir = new DirectoryInfo("c://tmp//ipfs-ext");

            var ctx = new IpfsInstanceContext(dir, new IpfsWin64())
            {
                Logger = logger
            };

            var ipfs = new IpfsInstance(ctx);

            ipfs.Daemon.Start();

            Console.ReadLine();

            ipfs.Daemon.Stop();
        }
    }
}

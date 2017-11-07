using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using Prime.Radiant.Components;
using Prime.Utility;

namespace Prime.Radiant.Components
{
    public class PublishManagerContext : CloudFlareContext
    {
        public Dispatcher Dispatcher { get; set; }

        public string SourcePath { get; set; }
        
        public ILogger L { get; set; }

        public string SshUri { get; set; }

        public string SshUsername { get; set; }

        public string SshPrivateKeyPath { get; set; }

        public string IpfsSeedPeerId { get; set; }

        public bool DoPrime { get; set; }

        public bool DoRadiant { get; set; }

        public static PublishManagerContext LoadDefault(Dispatcher dispatcher)
        {
            var path = Path.Combine(CommonFs.I.UserConfigDirectory.FullName, "publisher-context.txt");

            var f = File.ReadAllLines(path);
            if (f.Length != 10)
                return null;

            var pc = new PublishManagerContext
            {
                Dispatcher = dispatcher,
                SourcePath = f[0].Trim(),
                CloudFlareRootDomain = f[1].Trim(),
                CloudFlareZoneId = f[2].Trim(),
                CloudFlareEmail = f[3].Trim(),
                CloudFlareApiGlobal = f[4].Trim(),
                CloudFlareApiOrigin = f[5].Trim(),
                SshUri = f[6].Trim(),
                SshUsername = f[7].Trim(),
                SshPrivateKeyPath = f[8].Trim(),
                IpfsSeedPeerId = f[9].Trim()
            };

            return pc;
        }
    }
}
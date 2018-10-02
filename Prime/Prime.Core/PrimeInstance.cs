using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Prime.Base;
using Prime.Base.Messaging.Common;

namespace Prime.Core
{
    public class PrimeInstance : CommonBase
    {
        public readonly ExtensionManager ExtensionManager;

        public bool IsStarted { get; private set; }

        private readonly object _startLock = new object();

        public readonly CommandArgs CommandArgs = new CommandArgs();

        public PrimeInstance(PrimeContext c) : base(c)
        {
            ExtensionManager = new ExtensionManager(this, c);
        }

        public void Start()
        {
            L.Log($"Prime instance starting..");

            if (ExtensionManager == null)
            {
                L.Fatal("No extension manager was registered. Prime cannot start.");
                return;
            }

            lock (_startLock)
            {
                if (IsStarted)
                    return;
                IsStarted = true;
            }

            ExtensionManager.LoadInstallConfig();
            var exts = ExtensionManager.Instances.Select(s => s.Extension).ToList();

            exts.OfType<IExtensionInstanceCommandArgs>().ForEach(x => x.InitialiseCommandArgs(this, CommandArgs));
            exts.OfType<IExtensionCommandArgs>().ForEach(x => x.InitialiseCommandArgs(C, CommandArgs));
            exts.OfType<IExtensionStartup>().ForEach(x => x.PrimeStarted());

            L.Log($"Prime has registered {ExtensionManager.Instances.Count()} extension(s).");
            L.Log("Prime instance started.");
            L.Log("");
        }

        public void Stop()
        {
            lock (_startLock)
            {
                if (!IsStarted)
                    return;

                L.Log($"Prime instance stopping..");

                M.SendAsync(new PrimeShutdownNow());
                do
                {
                    Thread.Sleep(1);
                } while (ExtensionManager.Instances.OfType<IExtensionGracefullShutdown>().Any(x => !x.HasShutdown));

                IsStarted = false;
                M.SendAsync(new PrimeStopped());
            }
        }
    }
}

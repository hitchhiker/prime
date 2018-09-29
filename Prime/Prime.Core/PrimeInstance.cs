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

        private bool _isStarted;
        private readonly object _startLock = new object();

        public PrimeInstance(PrimeContext c) : base(c)
        {
            ExtensionManager = new ExtensionManager(this, c);
        }

        public void Start()
        {
            lock (_startLock)
            {
                if (_isStarted)
                    return;
                _isStarted = true;
            }

            ExtensionManager.LoadInstallConfig();
            ExtensionManager.Instances.Select(s => s.Extension).OfType<IExtensionStartup>().ForEach(x => x.PrimeStarted());
            L.Log($"Prime instance loaded {ExtensionManager.Instances.Count()} extensions.");
            L.Log("Prime instance started.");
        }

        public void Stop()
        {
            lock (_startLock)
            {
                if (!_isStarted)
                    return;

                M.SendAsync(new PrimeShutdownNow());
                do
                {
                    Thread.Sleep(1);
                } while (ExtensionManager.Instances.OfType<IExtensionGracefullShutdown>().Any(x => !x.HasShutdown));
            }
        }
    }
}

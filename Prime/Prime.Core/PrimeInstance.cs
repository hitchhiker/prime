using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prime.Core
{
    public class PrimeInstance
    {
        public readonly ExtensionManager ExtensionManager;

        public readonly PrimeContext C;

        private bool _isStarted;
        private readonly object _startLock = new object();

        public PrimeInstance(PrimeContext c)
        {
            C = c;
            ExtensionManager = new ExtensionManager(c);
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
        }
    }
}

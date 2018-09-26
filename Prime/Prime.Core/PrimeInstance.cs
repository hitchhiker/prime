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

        public PrimeInstance(PrimeContext c)
        {
            C = c;
            ExtensionManager = new ExtensionManager(c);
        }

        public void Start()
        {
            ExtensionManager.LoadInstallConfig();
            ExtensionManager.Instances.Select(s => s.Extension).OfType<IExtensionStartup>().ForEach(x => x.PrimeStarted());
        }
    }
}

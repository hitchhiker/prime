using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prime.Core
{
    public class PrimeInstance
    {
        public readonly ExtensionManager ExtensionManager;

        public readonly ServerContext Context;

        public PrimeInstance(ServerContext context)
        {
            Context = context;
            ExtensionManager = new ExtensionManager(context);
        }

        public void Start()
        {
            ExtensionManager.Instances.Select(s => s.Extension).OfType<IExtensionStartup>().ForEach(x => x.PrimeStarted());
        }
    }
}

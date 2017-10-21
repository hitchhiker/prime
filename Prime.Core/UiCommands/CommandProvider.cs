using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using Prime.Common;
using Prime.Common.Messages;
using Prime.Utility;

namespace Prime.Core
{
    internal class CommandProvider
    {
        private readonly IMessenger _messenger = DefaultMessenger.I.Default;
        private IReadOnlyList<CommandBase> _providers;
        public IReadOnlyList<CommandBase> Providers => _providers ?? (_providers = TypeCatalogue.I.ImplementInstances<CommandBase>().OrderByDescending(x => x.Priority).ToList());

        internal CommandBase Parse(string cstr)
        {
            if (string.IsNullOrWhiteSpace(cstr))
                return null;

            cstr = cstr.ToLower().Trim();
            
            var cm = Providers.Select(x=>x.Parse(cstr)).FirstOrDefault(x => x!=null);
            if (cm != null)
                return cm;

            //try shorter cuts
            cm = Providers.Select(x => x.Parse("a " + cstr)).FirstOrDefault(x => x != null);
            
            return cm;
        }
    }
}
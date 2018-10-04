using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Prime.Base;
using Prime.Core;

namespace Prime.NetCoreExtensionPackager
{
    public class CompositionTranslation : CommonBase
    {
        public CompositionTranslation(PrimeContext context, List<(Assembly, FileInfo)> possible) : base(context)
        {
            var findBase = possible.FirstOrDefault(x => string.Equals(x.Item2.Name, "Prime.Base.dll", StringComparison.OrdinalIgnoreCase));
            if (findBase == default)
            {
                L.Warn("Unable to find 'Prime.Base.dll', aborting.");
                return;
            }

            PrimeBaseAssembly = findBase.Item1;

            ExtensionIType = PrimeBaseAssembly.DefinedTypes.FirstOrDefault(x => x.FullName == typeof(IExtension).FullName);
            if (ExtensionIType == null)
            {
                L.Warn($"Unable to find remote type '{typeof(IExtension).Name}' in {findBase.Item2}, aborting.");
                return;
            }

            RemoteHelper = PrimeBaseAssembly.DefinedTypes.FirstOrDefault(x => x.FullName == typeof(CompositionHelper).FullName);
            if (RemoteHelper == null)
            {
                L.Warn($"Unable to find remote type '{typeof(CompositionHelper).Name}' in {findBase.Item2}, aborting.");
                return;
            }

            RemoteMethod = RemoteHelper.GetMethod(nameof(CompositionHelper.Serialised), BindingFlags.Public | BindingFlags.Static);
            if (RemoteMethod == null)
            {
                L.Warn($"Unable to find remote method '{nameof(CompositionHelper.Serialised)}' in {findBase.Item2}, aborting.");
                return;
            }

            Success = true;
        }

        public CompositionHelper GetHelper(Type t)
        {
            try
            {
                var i = Reflection.InstanceAny(t);
                var str = RemoteMethod.Invoke(null, new[] {i}) as string;
                return JsonConvert.DeserializeObject<CompositionHelper>(str);
            }
            catch
            {
                return null;
            }
        }

        public readonly bool Success;
        public readonly Type ExtensionIType;
        public readonly Assembly PrimeBaseAssembly;
        public readonly Type RemoteHelper;
        public readonly MethodInfo RemoteMethod;
    }
}
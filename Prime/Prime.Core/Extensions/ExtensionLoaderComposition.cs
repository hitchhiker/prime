using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Prime.Base;
using Prime.Core;

namespace Prime.Extensions
{
    public class ExtensionLoaderComposition : AssemblyLoadContext
    {
        [ImportMany]
        public IEnumerable<IExtension> ExtensionsComposed { get; private set; }

        [ImportMany]
        public IEnumerable<IExtensionExecute> ExtensionsExecuteComposed { get; private set; }

        public List<IExtension> ExtensionsAll { get; private set; }

        public void LoadExtensions(DirectoryInfo dir)
        {
            var files = dir.GetFiles("*.dll", SearchOption.AllDirectories).AsEnumerable();

            Assembly SafeLoadAssembly(FileInfo x)
            {
                try { return Default.LoadFromAssemblyPath(x.FullName); }
                catch { return null; }
            }

            var assemblies = files.Select(SafeLoadAssembly).Where(x=>x!=null).ToList();

            var types = new List<Type>();

            var extT = typeof(IExtension);

            foreach (var a in assemblies)
                types.AddRange(a.GetLoadableTypes().Where(x=> x.IsClass && !x.IsAbstract && !x.IsInterface && extT.IsAssignableFrom(x)).ToList());

            var configuration = new ContainerConfiguration();
            var asms = types.Select(x => x.Assembly).Distinct();

            configuration.WithAssemblies(asms);

            using (var container = configuration.CreateContainer())
            {
                ExtensionsComposed = container.GetExports<IExtension>();
                ExtensionsExecuteComposed = container.GetExports<IExtensionExecute>();
            }

            ExtensionsAll = ExtensionsComposed.Concat(ExtensionsExecuteComposed).ToList();
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            return null;
        }
    }
}

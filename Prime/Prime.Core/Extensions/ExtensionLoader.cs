using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Prime.Core
{
    public class ExtensionLoader
    {
        private readonly ExtensionManager _manager;

        [ImportMany]
        public IEnumerable<Lazy<IExtension>> Extensions { get; private set; }

        public ExtensionLoader(ExtensionManager manager)
        {
            _manager = manager;
        }

        public void LoadAllBinDirectoryAssemblies()
        {
            var di = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory;
            var asm = LoadAssemblies(di);
        }

        public T LoadExtension<T>(DirectoryInfo dir) where T : class, IExtension
        {
            var assemblies = LoadAssemblies(dir);
            if (assemblies.Count == 0)
                return default;

            var type = typeof(T);

            var typematches = assemblies.SelectMany(x => x.GetLoadableTypes()).Where(x=> x.IsAssignableStandard(type)).ToList();
            if (typematches.Count == 1)
                return Reflection.InstanceAny(typematches[0]) as T;

            return default;
        }

        public T LoadExtensionFromAssembly<T>(FileInfo file) where T : IExtension
        {
            var assembly = LoadAssembly(file);

            if (assembly == null)
                return default;

            return default;// assembly.GetLoadableTypes().FirstOrDefault(x => x == typeof(T));
        }

        public List<T> LoadExtensions<T>(DirectoryInfo dir)
        {
            var results = new List<T>();

            /*
            if (!dir.Exists)
                return results;

            var files = dir.GetFiles("*.dll");

            if (files == null || files.Length <= 0)
                return results;

            foreach (var fi in files)
            {
                var loaded = LoadExtensionFromAssembly<T>(fi);
                if (loaded?.Count > 0)
                    results.AddRange(loaded);
            }
            */
            return results;
        }

        public IReadOnlyList<Assembly> LoadAssemblies(DirectoryInfo path)
        {
           var usableAssemblies = new List<Assembly>();
            
            foreach (var dll in path.GetFiles("*.dll", SearchOption.AllDirectories))
            {
                if (dll.Name.Contains("roslyn", StringComparison.OrdinalIgnoreCase) || dll.DirectoryName.Contains("roslyn", StringComparison.OrdinalIgnoreCase))
                    return null;

                var a = LoadAssemblyLegacy(dll);
                if (a != null)
                    usableAssemblies.Add(a);
            }
            /*
            var configuration = new ContainerConfiguration();
            configuration.WithAssemblies(usableAssemblies);

            using (var container = configuration.CreateContainer())
            {
                Extensions = container.GetExports<Lazy<IExtension>>();
            }*/

            return usableAssemblies;
        }

        public Assembly LoadAssembly(FileInfo x)
        {
            try
            {
                return AssemblyLoadContext.Default.LoadFromAssemblyPath(x.FullName);
            }
            catch { return null; }
        }

        public static Assembly LoadAssemblyLegacy(FileInfo dll)
        {
            try
            {
                var a = Assembly.LoadFrom(dll.FullName);

                if (a.IsDynamic || a.GlobalAssemblyCache)
                    return null;
                /*
                if (a.FullName.StartsWith("Microsoft.", StringComparison.OrdinalIgnoreCase) || a.FullName.StartsWith("System.", StringComparison.OrdinalIgnoreCase) || a.FullName.StartsWith("NETStandard", StringComparison.OrdinalIgnoreCase))
                    return null;

                if (a.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright?.Contains("microsoft", StringComparison.OrdinalIgnoreCase) == true)
                    return null;
                    */
                return a;
            }
            catch (FileLoadException loadEx)
            {
                var x = loadEx;
            } // The Assembly has already been loaded.
            catch
            {
            } // If a BadImageFormatException exception is thrown, the file is not an assembly.
            return null;
        }
    }
}
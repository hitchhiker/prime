using Prime.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Prime.ExtensionPackager
{
    public class PackageMetaInspector
    {
        public static string ExtFileName = "prime-ext.json";

        public readonly ProgramContext Context;

        public PackageMetaInspector(ProgramContext context)
        {
            Context = context;
        }

        public Package Package { get; private set; }

        public void Inspect()
        {
            InspectDirectory(Context.SourceDirectory);
        }

        private void InspectDirectory(DirectoryInfo dir)
        {
            var fis = dir.GetFiles("*", SearchOption.AllDirectories).ToList();
            var extj = fis.FirstOrDefault(x => string.Equals(x.Name, ExtFileName, StringComparison.OrdinalIgnoreCase));

            if (extj == null)
                Context.Logger.Info("Cannot find '" + ExtFileName +"' in " + dir.FullName);
            
            var exts = FindExtensionDll(dir, fis);
            if (exts.Count == 0)
            {
               Context.Logger.Info("Cannot find any extensions in " + dir.FullName);
                return;
            }

            if (exts.Count > 1)
            {
               Context.Logger.Info("Found more than one extension in " + dir.FullName);
                return;
            }

            Package = exts[0];
            Package.AddStagingRange(fis.Where(x => !string.Equals(x.Name, ExtFileName, StringComparison.OrdinalIgnoreCase)));
        }

        private List<Package> FindExtensionDll(DirectoryInfo dir, List<FileInfo> files)
        {
            var r = new List<Package>();

            foreach (var fi in files.Where(x => x.Extension == ".dll"))
            {
                try
                {
                    if (!Context.IsPrime && fi.Name.StartsWith("prime.core.", StringComparison.OrdinalIgnoreCase))
                        continue;

                    var a = AssemblyLoadContext.Default.LoadFromAssemblyPath(fi.FullName);
                    var metaFi = LoadPossibleExtensionDll(a, dir, fi);
                    if (metaFi != null)
                        r.Add(metaFi);
                }
                catch (Exception e)
                {
                   Context.Logger.Info(e.Message + ": " + fi.FullName);
                }
            }

            return r;
        }

        private Package LoadPossibleExtensionDll(Assembly a, DirectoryInfo dir, FileInfo file)
        {
            if (a == null)
                return null;

            if (a.IsDynamic || a.GlobalAssemblyCache)
                return null;

            if (a.FullName.StartsWith("Microsoft.", StringComparison.OrdinalIgnoreCase) || a.FullName.StartsWith("System.", StringComparison.OrdinalIgnoreCase) || a.FullName.StartsWith("NETStandard", StringComparison.OrdinalIgnoreCase))
                return null;

            if (a.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright?.Contains("microsoft", StringComparison.OrdinalIgnoreCase) == true)
                return null;

            var extt = typeof(IExtension);

            var types = a.GetLoadableTypes().Where(x => !x.IsAbstract && extt.IsAssignableFrom(x)).ToList();

            if (types.Count == 0)
            {
                //logger.Info("Cannot find any type in " + file.FullName + " that implements " + extt);
                return null;
            }

            if (types.Count > 1)
            {
               Context.Logger.Info("Found multiple types in " + file.FullName + " implementing " + extt);
                return null;
            }

            var pm = types[0].InstanceAny<IExtension>();
            var meta = new PackageMeta(pm);
            var json = meta.ToJsonSimple();
            var fi = new FileInfo(Path.Combine(dir.FullName, ExtFileName));
            if (fi.Exists)
                fi.Delete();

            File.WriteAllText(fi.FullName, json);
            fi.Refresh();
            return new Package(meta, fi);
        }
    }
}
using Prime.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Prime.Base;

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
            var extj = fis.Where(x => string.Equals(x.Name, ExtFileName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

            if (extj == null)
                Context.Logger.Info("Currently no '" + ExtFileName + "' in " + dir.FullName);

            var exts = FindExtensionDll(dir, fis);
            if (exts.Count == 0)
            {
                Context.Logger.Warn("Cannot find any extensions in " + dir.FullName);
                return;
            }

            Package found = null;

            if (exts.Count > 1)
            {
                if (Context.ExtId != null)
                    found = exts.Where(x => x.Extension.Id == Context.ExtId).FirstOrDefault();

                if (found == null)
                {
                    Context.Logger.Error("Found more than one extension in " + dir.FullName);
                    return;
                }
            }
            else
                found = exts[0];

            Package = found;
            Package.AddStagingRange(fis.Where(x => !string.Equals(x.Name, ExtFileName, StringComparison.OrdinalIgnoreCase)));
        }

        private List<Package> FindExtensionDll(DirectoryInfo dir, List<FileInfo> files)
        {
            var r = new List<Package>();

            var possible = new List<(Assembly,FileInfo)>();

            foreach (var fi in files.Where(x => x.Extension == ".dll"))
            {
                try
                {
                    if (!Context.IsPrime &&
                        fi.Name.StartsWith("prime.core.", StringComparison.OrdinalIgnoreCase) ||
                        fi.Name.StartsWith("system.", StringComparison.OrdinalIgnoreCase) ||
                        fi.Name.StartsWith("microsoft.", StringComparison.OrdinalIgnoreCase))
                        continue;

                    var a = AssemblyLoadContext.Default.LoadFromAssemblyPath(fi.FullName);
                    possible.Add((a, fi));
                }
                catch (Exception e)
                {
                    if (!e.Message.Contains("Reference assemblies", StringComparison.OrdinalIgnoreCase))
                        Context.Logger.Info(e.Message + ": " + fi.FullName);
                }
            }

            foreach (var i in possible)
            {
                var metaFi = LoadPossibleExtensionDll(i.Item1, dir, i.Item2);
                if (metaFi != null)
                    r.Add(metaFi);
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

            Type foundType = null;

            if (types.Count > 1)
            {
               
                    Context.Logger.Info("Found multiple types in " + file.FullName + " implementing " + extt);
                return null;
            }

            foundType = types[0];

            var refrs = new List<Assembly>();
            var loaded = AppDomain.CurrentDomain.GetAssemblies().ToList();
            InspectReferences(a, refrs, loaded);

            var extRefs = GetExtensionReferences(refrs);

            var pm = foundType.InstanceAny<IExtension>();
            var meta = new PackageMeta(pm) {ExtensionReferences = extRefs};

            var json = meta.ToJsonSimple();
            var fi = new FileInfo(Path.Combine(dir.FullName, ExtFileName));
            if (fi.Exists)
                fi.Delete();

            File.WriteAllText(fi.FullName, json);
            fi.Refresh();
            return new Package(meta, fi);
        }

        public void InspectReferences(Assembly a, List<Assembly> assemblies, List<Assembly> loaded)
        {
            foreach (var r in a.GetReferencedAssemblies())
            {
                var ra = loaded.Where(x => x.FullName == r.FullName).FirstOrDefault();
                if (ra == null)
                    continue;
                if (ra.IsDynamic || ra.GlobalAssemblyCache || assemblies.Contains(ra))
                    continue;

                assemblies.Add(ra);
                InspectReferences(ra, assemblies, loaded);
            }
        }

        public List<ObjectId> GetExtensionReferences(List<Assembly> references)
        {
            var extt = typeof(IExtension);
            var list = new List<ObjectId>();
            foreach (var a in references)
            {
                var types = a.GetLoadableTypes().Where(x => !x.IsAbstract && extt.IsAssignableFrom(x)).ToList();
                if (types.Count != 1)
                    continue;
                var pm = types[0].InstanceAny<IExtension>();
                list.Add(pm.Id);
            }
            return list;
        }
    }
}
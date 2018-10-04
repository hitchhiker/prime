using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Prime.Base;
using Prime.Core;

namespace Prime.NetCoreExtensionPackager
{
    public class PackageMetaInspector
    {
        public static string ExtFileName = "prime-ext.json";

        public readonly NetCorePackagerContext Context;

        public PackageMetaInspector(NetCorePackagerContext context)
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

            //if (extj == null)
            //    Context.L.Info("Currently no '" + ExtFileName + "' in " + dir.FullName);

            var exts = FindExtensionsFromDirectory(dir, fis);
            if (exts.Count == 0)
            {
                Context.L.Warn("Cannot find any extensions in " + dir.FullName);
                return;
            }

            Package found = null;

            if (exts.Count > 1)
            {
                if (Context.ExtId != null)
                    found = exts.FirstOrDefault(x => x.PackageMeta.Id == Context.ExtId);
                else if (exts.Count==2)
                    found = exts.FirstOrDefault(x => x.Name != "prime");
                
                if (found == null)
                {
                    Context.L.Error("Cant determine which package to bundle: " + string.Join(", ", exts.Select(x=>x.Name)));
                    return;
                }
            }
            else
                found = exts[0];

            DiscoverReferences(found);

            Package = found;
            Package.AddStagingRange(fis.Where(x => !string.Equals(x.Name, ExtFileName, StringComparison.OrdinalIgnoreCase)));
        }

        private void DiscoverReferences(Package found)
        {
            var refrs = new List<Assembly>();
            var loaded = AppDomain.CurrentDomain.GetAssemblies().ToList();
            InspectReferences(found.Assembly, refrs, loaded);
            var extRefs = GetAssemblyPackageReferences(refrs);
            found.AssemblyReferences.AddRange(refrs.Where(r => !extRefs.Any(x => x.Assembly.Equals(r))).Select(n=> new PackageAssemblyReference(n)));
            found.PackageMeta.PackageReferences = extRefs;
            found.WriteFile();
        }

        private List<Package> FindExtensionsFromDirectory(DirectoryInfo dir, List<FileInfo> files)
        {
            var foundPackages = new List<Package>();

            var possible = new List<(Assembly, FileInfo)>();

            foreach (var fi in files.Where(x => x.Extension == ".dll"))
            {
                try
                {
                    if (fi.Name.StartsWith("system.", StringComparison.OrdinalIgnoreCase) ||
                        fi.Name.StartsWith("microsoft.", StringComparison.OrdinalIgnoreCase))
                        continue;

                    var a = Assembly.LoadFrom(fi.FullName);
                    possible.Add((a, fi));
                }
                catch (Exception e)
                {
                    if (!e.Message.Contains("Reference assemblies", StringComparison.OrdinalIgnoreCase))
                        Context.L.Info(e.Message + ": " + fi.FullName);
                }
            }

            foreach (var i in possible)
            {
                if (i.Item2.FullName.Contains("dotnet" + Path.DirectorySeparatorChar + "shared"))
                    continue;

                var package = LoadPossibleExtensionDll(i.Item1, dir, i.Item2);
                if (package != null)
                    foundPackages.Add(package);
            }

            return foundPackages;
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

            var foundType = DetermineExtension(a, file);
            if (foundType == null || foundType == typeof(PackageMeta))
                return null;

            var pm = foundType.InstanceAny<IExtension>();
            var meta = new PackageMeta(pm, a.GetName().Version);

            if (!Context.IsBase && meta.Id == "prime:base".GetObjectIdHashCode())
                return null;

            var metaInfo = new FileInfo(Path.Combine(dir.FullName, ExtFileName));
            
            return new Package(a, meta, metaInfo);
        }

        private Type DetermineExtension(Assembly a, FileInfo file)
        {
            var extt = typeof(IExtension);
            var pmType = typeof(PackageMeta);

            var types = a.GetLoadableTypes().Where(x => !x.IsAbstract && extt.IsAssignableFrom(x) && x != pmType).ToList();

            if (types.Count <= 1)
                return types.FirstOrDefault();

            if (Context.ExtId != null)
            {
                var type = types.FirstOrDefault(x => x.InstanceAny<IExtension>()?.Id == Context.ExtId);
                if (type != null)
                    return type;
            }

            Context.L.Warn("Found multiple types in '" + file.FullName + "' implementing '" + extt + "'. You can specify the extension if required.");
            return null;
        }

        public void InspectReferences(Assembly a, List<Assembly> assemblies, List<Assembly> loaded)
        {
            foreach (var r in a.GetReferencedAssemblies())
            {
                var ra = loaded.FirstOrDefault(x => x.FullName == r.FullName);
                if (ra == null)
                    continue;

                if (ra.IsDynamic || ra.GlobalAssemblyCache || assemblies.Contains(ra))
                    continue;

                if (ra.Location.Contains("dotnet" + Path.DirectorySeparatorChar + "shared"))
                    continue;

                assemblies.Add(ra);
                InspectReferences(ra, assemblies, loaded);
            }
        }

        public List<PackageReference> GetAssemblyPackageReferences(List<Assembly> references)
        {
            var extt = typeof(IExtension);
            var list = new List<PackageReference>();
            foreach (var a in references)
            {
                var types = a.GetLoadableTypes().Where(x => !x.IsAbstract && extt.IsAssignableFrom(x)).ToList();
                if (types.Count != 1)
                    continue;
                var pm = types[0].InstanceAny<IExtension>();
                if (pm.Id != null)
                    list.Add(new PackageReference(pm) {Assembly = a});
            }
            return list;
        }
    }
}
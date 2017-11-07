using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Prime.Radiant.Components
{
    public class PackageFiller
    {
        private readonly PublishManager _publishManager;

        public PackageFiller(PublishManager publishManager)
        {
            _publishManager = publishManager;
        }

        public PackageBundler Fill(PackageBundler packageBundler)
        {
            _publishManager.L.Info("Packaging: " + packageBundler.ProjectKey);

            var packages = new List<Package>();

            packages.Add(packageBundler.CreatePackage("core", files =>
            {
                var result = new List<FileInfo>();
                result.AddRange(files.Where(x => x.Name.StartsWith("prime", StringComparison.OrdinalIgnoreCase)));
                result.AddRange(files.Where(x => x.Name.StartsWith("radiant", StringComparison.OrdinalIgnoreCase)));
                result.AddRange(files.Where(x => x.Name.StartsWith("spark", StringComparison.OrdinalIgnoreCase)));
                return result;
            }));

            packages.Add(packageBundler.CreatePackage("ext-ipfsnative", files =>
            {
                var result = new List<FileInfo>(files.Where(x=>x.Name.StartsWith("ipfs-x") && x.Name.EndsWith(".exe")));
                return result;
            }));

            packages.Add(packageBundler.CreatePackage("ext-7znative", files =>
            {
                var result = new List<FileInfo>(files.Where(x => x.Name.StartsWith("7z-x") && x.Name.EndsWith(".dll")));
                return result;
            }));

            packages.Add(packageBundler.CreatePackage("ext-ms", files =>
            {
                var result = Except(packages, new List<FileInfo>(files));
                result.RemoveAll(x => !(x.Name.StartsWith("common.", StringComparison.OrdinalIgnoreCase) ||
                                        x.Name.StartsWith("system.", StringComparison.OrdinalIgnoreCase) ||
                                        x.Name.StartsWith("microsoft.", StringComparison.OrdinalIgnoreCase)));
                return result;
            }));

            packages.Add(CreateMod(packageBundler, 0, packages));
            packages.Add(CreateMod(packageBundler, 1, packages));
            packages.Add(CreateMod(packageBundler, 2, packages));
            packages.Add(CreateMod(packageBundler, 3, packages));
            packages.Add(CreateMod(packageBundler, 4, packages));
            packages.Add(CreateMod(packageBundler, 5, packages));

            return packageBundler;
        }

        public Package CreateMod(PackageBundler packageBundler, int mod6, List<Package> alreadyDone)
        {
            return packageBundler.CreatePackage("mod-" + mod6, files =>
            {
                var result = Except(alreadyDone, new List<FileInfo>(files));
                result.RemoveAll(x => Math.Abs(x.Name.GetHashCode()) % 6 != mod6);
                return result;
            });
        }

        private static List<FileInfo> Except(List<Package> alreadyDone, List<FileInfo> result)
        {
            foreach (var pack in alreadyDone)
                result.RemoveAll(x => pack.Any(p => p.FullName == x.FullName));

            return result;
        }
    }
}
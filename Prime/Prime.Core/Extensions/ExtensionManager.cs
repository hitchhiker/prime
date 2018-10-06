using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Prime.Base;
using Prime.Core.Extensions;
using Prime.Extensions;

namespace Prime.Core
{
    public class ExtensionManager : CommonBase
    {
        public readonly PrimeInstance Prime;
        public readonly PrimeContext Context;
        public readonly ConfigPackageNode PackageConfig;
        public readonly TypeCatalogue Types;
        public readonly AssemblyCatalogue Assemblies;
        public readonly ExtensionList Instances;
        public readonly InstallConfig InstallConfig;

        public ExtensionManager(PrimeInstance prime, PrimeContext context) : base(prime)
        {
            Prime = prime;
            Context = context;
            PackageConfig = Context.Config.ConfigPackageNode;
            InstallConfig = PackageConfig.InstallConfig;
            Instances = new ExtensionList(this);
            Assemblies = context.Assemblies;
            Types = context.Types;
        }

        public void LoadInstallConfig()
        {
            if (Context.DllLocal)
                L.Log("[DEVELOPMENT ONLY] Loading extensions from the executing application's directory.");

            EnsureCoreExtensions();

            if (!C.DllLocal)
                new PrimeUpgrade(this).CheckInstallVersions();

            var exts = C.DllLocal
                ? InstallConfig.Installs
                : InstallConfig.Installs.Where(x => x.Version != null);

            foreach (var i in exts)
                Load<IExtension>(i, Context.DllLocal);
        }

        public void SaveConfig()
        {
            if (C.Config.ConfigLoadedFrom == null)
                return;

            C.Config.Save(C.Config.ConfigLoadedFrom, true);
        }

        private void EnsureCoreExtensions()
        {
            if (InstallConfig.Installs.All(x => x.Id != PrimeBaseExtension.StaticId))
                InstallConfig.Installs.Add(new InstallEntry() {IdString = PrimeBaseExtension.StaticId.ToString()});

            if (InstallConfig.Installs.All(x => x.Id != PrimeCoreExtension.StaticId))
                InstallConfig.Installs.Add(new InstallEntry() {IdString = PrimeCoreExtension.StaticId.ToString()});

            foreach (var ext in new List<IExtension>(){new PrimeCoreExtension(), new PrimeBaseExtension()})
            {
                Instances.Init(ext);
                Context.AddInitialisedExtension(ext);
            }
        }

        public T Load<T>(InstallEntry p, bool fromAppDir = false) where T : class, IExtension
        {
            if (p.Id == PrimeBaseExtension.StaticId || p.Id == PrimeCoreExtension.StaticId)
                return null;

            var ext = Instances.PreCheck<T>(p.Id);
            if (ext != null)
                return ext;

            var dir = fromAppDir ? new FileInfo(Assembly.GetExecutingAssembly().Location).Directory : Instances.GetPackageDirectory(p.Id, p.Version);

            if (dir == null)
            {
                L.Fatal("Can't find directory for package: " + p.Id);
                return default;
            }

            var loaded = LoadExtension<T>(p, dir);
            if (loaded == null)
            {
                L.Fatal("Can't load extension " + p.Id + " for platform '" + Context.PlatformCurrent + "' from directory: " + dir);
                return default;
            }

            Instances.Init(loaded);
            Context.AddInitialisedExtension(loaded);
            L.Info($"Extension \'{loaded.Title} {loaded.GetType().Assembly.GetName().Version} {(loaded as IExtensionPlatform)?.Platform}\' loaded from {dir.FullName}.");
            return loaded;
        }

        private T LoadExtension<T>(InstallEntry p, DirectoryInfo dir) where T : class, IExtension
        {
            var loader = new ExtensionLoaderComposition();
            loader.LoadExtensions(dir);

            var exts = loader.ExtensionsAll.Where(x => x.Id == p.Id).ToList();

            if (exts.Count == 0)
                return default;

            if (exts.Count == 1 && exts[0] is IExtensionPlatform pl && pl.Platform == Platform.NotSpecified)
                return exts[0] as T;

            if (exts.Count == 1 && !(exts[0] is IExtensionPlatform))
                return exts[0] as T;

            return exts.OfType<IExtensionPlatform>().FirstOrDefault(x => x.Platform == Context.PlatformCurrent) as T;
        }
    }
}
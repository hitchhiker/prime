using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Prime.Base;
using Prime.Extensions;

namespace Prime.Core
{
    public class ExtensionManager
    {
        public readonly PrimeInstance Prime;
        public readonly PrimeContext Context;
        public readonly ConfigPackageNode PackageConfig;
        public readonly TypeCatalogue Types;
        public readonly AssemblyCatalogue Assemblies;
        public readonly ExtensionList Instances;
        public readonly InstallConfig InstallConfig;

        public ExtensionManager(PrimeInstance prime, PrimeContext context)
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
                Context.L.Log("[DEVELOPMENT ONLY] Loading extensions from the executing application's directory.");

            if (InstallConfig.Installs.All(x => x.Id != PrimeBaseExtension.StaticId))
                InstallConfig.Installs.Add(new InstallEntry() {IdString = PrimeBaseExtension.StaticId.ToString()});
           
            if (InstallConfig.Installs.All(x => x.Id != PrimeCoreExtension.StaticId))
                InstallConfig.Installs.Add(new InstallEntry() {IdString = PrimeCoreExtension.StaticId.ToString()});

            foreach (var i in InstallConfig.Installs)
                Load<IExtension>(i, Context.DllLocal);

            CheckInstallVersions();
        }

        private void CheckInstallVersions()
        {
            var changed = false;

            var missing = InstallConfig.Installs.Where(x => x.Version == null).ToList();

            foreach (var p in missing)
                changed = changed | CheckInstallVersion(p);

            if (!changed || Context.Config.ConfigLoadedFrom == null)
                return;

            Context.L.Info("Updating main config with current versions for installed packages.");
            Context.Config.Save(Context.Config.ConfigLoadedFrom);
        }

        public bool UpgradeInstallVersions()
        {
            var changed = false;

            foreach (var p in InstallConfig.Installs)
                changed = changed | UpgradeInstallVersion(p);

            var core = new InstallEntry() {IdString = PrimeCoreExtension.StaticId.ToString()};
            UpgradeInstallVersion(core);

            if (InstallConfig.EntryPointCore != core.VersionString && !string.IsNullOrWhiteSpace(core.VersionString))
            {
                InstallConfig.EntryPointCore = core.VersionString;
                changed = true;
            }
            if (!changed)
                return false;

            Context.L.Info("Updating main config with latest versions for installed packages.");
            Context.Config.Save(Context.Config.ConfigLoadedFrom);
            return true;
        }

        private bool CheckInstallVersion(InstallEntry p)
        {
            var loaded = Instances.FirstOrDefault(x => x.IsLoaded && x.Id == p.Id);
            if (loaded == null)
                return false;

            p.VersionString = loaded.Version.ToString();
            return true;
        }

        private bool UpgradeInstallVersion(InstallEntry p)
        {
            var dir = Instances.GetPackageDirectory(p.Id);
            if (dir == null)
                return false;

            var fi = new FileInfo(Path.Combine(dir.FullName, "prime-ext.json"));
            var pm = PackageMeta.From(fi);
            if (pm == null)
                return false;

            if (p.Version == pm.Version)
                return false;

            p.VersionString = pm.Version.ToString();

            return true;
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
                Context.L.Fatal("Can't find directory for package: " + p.Id);
                return default;
            }

            var loaded = LoadExtension<T>(p, dir);
            if (loaded == null)
            {
                Context.L.Fatal("Can't load extension " + p.Id + " for platform '" + Context.PlatformCurrent + "' from directory: " + dir);
                return default;
            }

            Instances.Init(loaded);
            Context.AddInitialisedExtension(loaded);
            Context.L.Info($"Extension \'{loaded.Title} {loaded.Version} {(loaded as IExtensionPlatform)?.Platform}\' loaded from {dir.FullName}.");
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
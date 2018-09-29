using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Prime.Base;
using Prime.Extensions;

namespace Prime.Core
{
    public class ExtensionManager
    {
        public readonly PrimeContext Context;
        public readonly PackageConfig Config;
        public readonly TypeCatalogue Types;
        public readonly AssemblyCatalogue Assemblies;
        public readonly ExtensionList Instances;

        public ExtensionManager(PrimeContext context)
        {
            Context = context;
            Config = Context.Config.PackageConfig;
            Instances = new ExtensionList(this);
            Assemblies = context.Assemblies;
            Types = context.Types;
        }

        public void LoadInstallConfig()
        {
            foreach (var i in Config.InstallConfig.Installs)
                Load<IExtension>(i.Id);
        }

        public T Load<T>(ObjectId id) where T : class, IExtension
        {
            var ext = Instances.PreCheck<T>(id);
            if (ext != null)
                return ext;

            var dir = Instances.GetPackageDirectory(id);
            if (dir == null)
            {
                Context.L.Fatal("Can't find directory for package: " + id);
                return default;
            }

            var loaded = LoadExtension<T>(id, dir);
            if (loaded == null)
            {
                Context.L.Fatal("Can't load extension " + id + " from directory: " + dir);
                return default;
            }

            Instances.Init(loaded);
            Context.AddInitialisedExtension(loaded);
            Context.L.Info($"Extension \'{loaded.Title} {loaded.Version} {(loaded as IExtensionPlatform)?.Platform}\' loaded.");
            return loaded;
        }

        private T LoadExtension<T>(ObjectId id, DirectoryInfo dir) where T : class, IExtension
        {
            var loader = new ExtensionLoaderComposition();
            loader.LoadExtensions(dir);

            var exts = loader.ExtensionsAll.Where(x => x.Id == id).ToList();

            if (exts.Count == 0)
                return default;

            if (exts.Count == 1 && exts[0] is IExtensionPlatform p && p.Platform == Platform.NotSpecified)
                return exts[0] as T;

            if (exts.Count == 1 && !(exts[0] is IExtensionPlatform))
                return exts[0] as T;

            return exts.OfType<IExtensionPlatform>().FirstOrDefault(x => x.Platform == Context.PlatformCurrent) as T;
        }
    }
}
using System.Collections;
using System.IO;
using System.Linq;
using Prime.Base;

namespace Prime.Core
{
    public class ExtensionManager : UniqueList<ExtensionInstance>
    {
        public readonly ServerContext Context;
        public readonly PackageConfig Config;
        public readonly TypeCatalogue Types;
        public readonly AssemblyCatalogue Assemblies;
        public readonly ExtensionLoader Loader;
        public readonly ExtensionList ExtensionList;

        public ExtensionManager(ServerContext context)
        {
            Context = context;
            Config = Context.Config.PackageConfig;
            Loader = new ExtensionLoader(this);
            ExtensionList = new ExtensionList(this);
            Assemblies = context.Assemblies;
            Types = context.Types;
        }

        public T Load<T>(ObjectId id) where T : class, IExtension
        {
            var ext = ExtensionList.PreCheck<T>(id);
            if (ext != null)
                return ext;

            var dir = ExtensionList.GetPackageDirectory(id);
            if (dir == null)
                return default; //TODO: Include real catalogue
            
            ext = (T)Loader.LoadExtension<T>(dir);
            if (ext != null)
                ExtensionList.Init(ext);

            return ext;
        }
    }
}
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Prime.Base;

namespace Prime.Core
{
    public class ExtensionList
    {
        private readonly ExtensionManager _manager;
        private readonly UniqueList<ExtensionInstance> _exts = new UniqueList<ExtensionInstance>();
        private readonly object _lock = new object();

        public ExtensionList(ExtensionManager manager)
        {
            _manager = manager;
        }

        public T PreCheck<T>(ObjectId id) where T : IExtension
        {
            lock (_lock)
            {
                var ext = _exts.Where(x => x.Id == id).OrderByDescending(x => x.Version).Select(x=>x.Extension).OfType<T>().FirstOrDefault();
                if (ext != null)
                    return ext;
            }
            return default;
        }

        public void Init(IExtension ext)
        {
            if (ext is IExtensionExecute ex)
                ex.Main(_manager.Context);

            lock(_lock)
                _exts.Add(new ExtensionInstance(ext));
        }

        public DirectoryInfo GetPackageDirectory(ObjectId extensionId)
        {
            var config = _manager.Config;

            if (config.InstallConfig.Installs.All(x => x.Id != extensionId))
                return null;

            var redirectPath = GetPackageRedirection(extensionId);
            if (redirectPath != null)
                return redirectPath;

            var iDir = _manager.Context.FileSystem.InstallDirectory;
            var dirs = iDir.GetDirectories("*-" + extensionId, SearchOption.TopDirectoryOnly).ToList();
            if (dirs.Count != 1)
                return null;

            return dirs.First();
        }

        public DirectoryInfo GetPackageRedirection(ObjectId extensionId)
        {
            var redirectPath = _manager.Config.RedirectConfig.Redirects.FirstOrDefault(x => x.Id == extensionId)?.Path;
            return redirectPath != null ? new DirectoryInfo(redirectPath.GetFullPath(_manager.Context.ConfigDirectoryInfo)) : null;
        }
    }
}
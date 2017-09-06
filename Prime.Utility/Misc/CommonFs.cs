using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Utility
{
    public class CommonFs
    {
        private CommonFs()
        {
            PrimeUsrDirectory = GetLocalApp();
        }

        public static CommonFs I => Lazy.Value;
        private static readonly Lazy<CommonFs> Lazy = new Lazy<CommonFs>(()=>new CommonFs());

        public DirectoryInfo PrimeUsrDirectory { get; private set; }

        private DirectoryInfo _userConfigDirectory;
        public DirectoryInfo UserConfigDirectory => _userConfigDirectory ?? (_userConfigDirectory = GetCreateUsrSubDirectory("config"));

        private DirectoryInfo GetLocalApp()
        {
            var di = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Prime"));
            if (!di.Exists)
                di.Create();

            return di;
        }

        public DirectoryInfo GetCreateUsrSubDirectory(string directoryName)
        {
            var di = new DirectoryInfo(Path.Combine(PrimeUsrDirectory.FullName, directoryName));
            if (!di.Exists)
                di.Create();

            return di;
        }

        public DirectoryInfo GetCreateSubDirectory(DirectoryInfo dir, string directoryName)
        {
            var di = new DirectoryInfo(Path.Combine(dir.FullName, directoryName));
            if (!di.Exists)
                di.Create();

            return di;
        }
    }
}

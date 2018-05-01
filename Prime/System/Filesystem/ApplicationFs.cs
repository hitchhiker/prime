using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Core
{
    public class ApplicationFs
    {
        private ApplicationFs()
        {
            PrimeDirectory = GetPrimeDirectory();
        }

        public static ApplicationFs I => Lazy.Value;
        private static readonly Lazy<ApplicationFs> Lazy = new Lazy<ApplicationFs>(()=>new ApplicationFs());

        public DirectoryInfo PrimeDirectory { get; private set; }

        private DirectoryInfo _userConfigDirectory;
        public DirectoryInfo ConfigDirectory => _userConfigDirectory ?? (_userConfigDirectory = GetCreatePrimeSubDirectory("config"));

        private DirectoryInfo GetPrimeDirectory()
        {
            var di = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
            return di.EnsureSubDirectory("Prime");
        }

        public DirectoryInfo GetCreatePrimeSubDirectory(string directoryName)
        {
            return PrimeDirectory.EnsureSubDirectory(directoryName);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prime.Common;

namespace Prime.Utility.Win
{
    public class PrimeWindows : IPrimeEnvironment
    {
        private DirectoryInfo _storageDirectory;

        public DirectoryInfo StorageDirectory => _storageDirectory ?? (_storageDirectory = GetSDir());

        private DirectoryInfo GetSDir()
        {
            var di = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Prime"));
            if (!di.Exists)
                di.Create();
            return di;
        }
    }
}

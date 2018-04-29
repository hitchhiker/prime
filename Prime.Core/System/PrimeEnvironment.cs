using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Prime.Core;

namespace Prime.Core
{
    public class PrimeAnyOs : IPrimeEnvironment
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

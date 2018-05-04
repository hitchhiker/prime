using Prime.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Prime.PackageManager
{
    public class PackageManagerContext
    {
        public DirectoryInfo WorkspaceDirectory { get; set; }

        private DirectoryInfo _catalogueDirectory;
        public DirectoryInfo CatalogueDirectory => _catalogueDirectory ?? (_catalogueDirectory = WorkspaceDirectory.EnsureSubDirectory("catalogue"));

        private DirectoryInfo _distributionDirectory;
        public DirectoryInfo DistributionDirectory => _distributionDirectory ?? (_distributionDirectory = WorkspaceDirectory.EnsureSubDirectory("dist"));

        public bool IsPrime { get; set; }

        public ILogger Logger { get; set; }
    }
}

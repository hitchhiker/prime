using System;
using System.IO;
using System.Windows;
using System.Xml.Serialization;

namespace Prime.Core
{
    public class PrimeServerConfig : XmlConfigBase
    {
        private static string FileName = "prime-server.config";

        private PrimeServerConfig() {}

        public static PrimeServerConfig Get(string path)
        {
            var fi = new FileInfo(path);
            if (!fi.Exists)
            {
                if (!TryDefault(fi) || !fi.Exists)
                    throw new Exception(fi.FullName + " does not exist, and no .default file present.");
            }

            var pc = Load<PrimeServerConfig>(new FileInfo(path));
            if (pc != null)
                pc.ConfigLoadedFrom = fi;

            return pc ?? new PrimeServerConfig();
        }

        private static bool TryDefault(FileInfo fi)
        {
            var dfi = new FileInfo(fi.FullName + ".default");
            if (!dfi.Exists)
                return false;

            dfi.CopyTo(fi.FullName, true);
            fi.Refresh();
            return true;
        }

        public void Save(DirectoryInfo directory)
        {
            Save(GetFileInfo(directory, FileName));
        }

        [XmlIgnore]
        public FileInfo ConfigLoadedFrom { get; private set; }

        [XmlElement("path")]
        public string BasePath { get; set; }

        [XmlElement("packages")]
        public PackageConfig PackageConfig { get; set; } = new PackageConfig();

    }
}
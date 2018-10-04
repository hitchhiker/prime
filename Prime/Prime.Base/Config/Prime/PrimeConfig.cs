using System;
using System.IO;
using System.Windows;
using System.Xml.Serialization;

namespace Prime.Core
{
    [XmlRoot("config")]
    public class PrimeConfig : XmlConfigBase
    {
        private PrimeConfig() {}

        public static PrimeConfig Get(string path)
        {
            var fi = new FileInfo(path);
            if (!fi.Exists)
            {
                if (!TryDefault(fi) || !fi.Exists)
                    throw new FileNotFoundException(fi.FullName + " does not exist, and no .default file present.");
            }

            var pc = Load<PrimeConfig>(new FileInfo(path));
            if (pc != null)
                pc.ConfigLoadedFrom = fi;

            return pc ?? new PrimeConfig();
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
            if (ConfigLoadedFrom == null)
                return;

            Save(GetFileInfo(directory, ConfigLoadedFrom.Name));
        }

        [XmlIgnore]
        public FileInfo ConfigLoadedFrom { get; private set; }

        [XmlElement("path")]
        public string BasePath { get; set; }

        [XmlElement("entry")]
        public string Entry { get; set; }

        [XmlElement("packages")]
        public ConfigPackageNode ConfigPackageNode { get; set; } = new ConfigPackageNode();

        [XmlElement("catalogue")]
        public CatalogueConfig CatalogueConfig { get; set; } = new CatalogueConfig();

        [XmlElement("nugetPath")]
        public string NugetPath { get; set; } = "[USER]//.nuget";
    }
}
using System;
using System.IO;
using System.Xml.Serialization;

namespace Prime.Core
{
    [Serializable]
    public abstract class XmlConfigBase
    {
        public static T Load<T>(FileInfo fileInfo) where T : XmlConfigBase
        {
            T config;

            if (!fileInfo.Exists)
                return default;

            using (var stream = File.OpenRead(fileInfo.FullName))
                config = new XmlSerializer(typeof(T)).Deserialize(stream) as T;
            
            return config;
        }

        public void Save(FileInfo fileInfo)
        {
            using (var stream = new FileStream(fileInfo.FullName, FileMode.Create))
                new XmlSerializer(GetType()).Serialize(stream, this);
        }

        public static FileInfo GetFileInfo(DirectoryInfo directory, string name)
        {
            return new FileInfo(Path.Combine(directory.FullName, name));
        }
    }
}
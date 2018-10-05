using System;
using System.IO;
using System.Xml.Serialization;
using Prime.Base;

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

        public void Save(FileInfo fileInfo, bool keepOld = false)
        {
            if (keepOld)
                KeepOld(fileInfo);

            using (var stream = new FileStream(fileInfo.FullName, FileMode.Create))
                new XmlSerializer(GetType()).Serialize(stream, this);
        }

        public static FileInfo GetFileInfo(DirectoryInfo directory, string name)
        {
            return new FileInfo(Path.Combine(directory.FullName, name));
        }

        private static void KeepOld(FileInfo file)
        {
            if (!file.Exists)
                return;

            var dir = file.Directory;
            var sub = dir.CreateSubdirectory("config_old");
            file.CopyTo(Path.Combine(sub.FullName, file.Name + "." + ObjectId.NewObjectId() + ".backup"));
        }
    }
}
using System;
using System.IO;
using System.Xml.Serialization;

namespace prime
{
    [Serializable]
    public abstract class XmlConfigBase
    {
        public static T Load<T>(string path) where T : XmlConfigBase
        {
            T config;

            using (var stream = File.OpenRead(path))
                config = new XmlSerializer(typeof(T)).Deserialize(stream) as T;
            
            return config;
        }

        public void Save<T>(string path) where T : XmlConfigBase
        {
            using (var stream = new FileStream(path, FileMode.Create))
                new XmlSerializer(typeof(T)).Serialize(stream, this);
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Prime.Base;

namespace Prime.Core
{
    public abstract class JsonConfigBase<T> where T : JsonConfigBase<T>
    {
        [JsonIgnore]
        public string LoadedFromPath { get; private set; }

        public void Save(Formatting formatting = Formatting.Indented)
        {
            if (string.IsNullOrWhiteSpace(LoadedFromPath))
                throw new Exception($"You cant use the parameterless {nameof(Save)} method, as this object was not previously loaded from the filesystem.");

            Save(new FileInfo(LoadedFromPath), formatting);
        }

        public void Save(FileInfo destination, Formatting formatting = Formatting.Indented)
        {
            if (destination.Exists)
                destination.Delete();

            var txt = JsonConvert.SerializeObject(this, formatting);
            File.WriteAllText(destination.FullName, txt);
        }

        public static T Get(CommonBase c, FileInfo file, bool ignoreException = true)
        {
            return Get(c.C, file, ignoreException);
        }

        public static T Get(CommonBase c, string path, bool ignoreException = true)
        {
            return Get(c.C, path, ignoreException);
        }

        public static T Get(PrimeContext c, FileInfo file, bool ignoreException = true)
        {
            if (file.Exists)
                return GetConfig(c, file.FullName, ignoreException);

            if (ignoreException)
                return default;
            
            throw new Exception("'" + file.FullName + "' does not exist in " + typeof(T).FullName);
        }

        public static T Get(PrimeContext c, string path, bool ignoreException = true)
        {
            path = path.ResolveSpecial();

            if (File.Exists(path))
                return GetConfig(c, path, ignoreException);

            if (ignoreException) 
                return default;
            
            throw new Exception("'" + path + "' does not exist in " + typeof(T).FullName);
        }

        private static T GetConfig(PrimeContext c, string path, bool ignoreException)
        {
            var txt = File.ReadAllText(path);
            T o;
            try
            {
                o = JsonConvert.DeserializeObject<T>(txt);
                o.LoadedFromPath = path;
            }
            catch
            {
                c.L.Warn("Unable to deserialise: " + path);
                if (ignoreException)
                    return default;
                throw;
            }
            return o;
        }
    }
}

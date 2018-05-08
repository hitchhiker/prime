using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Prime.Core
{
    public class ExtensionLoader
    {
        public List<T> LoadExtensions<T>(string path)
        {
            var results = new List<T>();

            var dir = new DirectoryInfo(path);
            if (!dir.Exists)
                return results;
            
            var files = dir.GetFiles("*.dll");

            if (files == null || files.Length <= 0)
                return results;

            foreach (var fi in files)
            {
                var loaded = LoadExtensionFromAssembly<T>(fi.FullName);
                if (loaded?.Count > 0)
                    results.AddRange(loaded);
            }

            return results;
        }

        private List<T> LoadExtensionFromAssembly<T>(string filePath)
        {
            var results = new List<T>();

            var extType = typeof(T);

            var assembly = Assembly.LoadFrom(filePath);

            if (assembly == null)
                return results;
            
            var types = assembly.GetExportedTypes();

            foreach (var t in types)
            {
                if (!t.IsClass || t.IsNotPublic || t.IsAbstract || !extType.IsAssignableFrom(t))
                    continue;

                var ext = (T)Activator.CreateInstance(t);
                if (ext != null)
                    results.Add(ext);
            }

            return results;
        }
    }
}
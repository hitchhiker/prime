using System.IO;
using System.Reflection;

namespace Prime.ExtensionPackager
{
    public class PackageAssemblyReference
    {
        public PackageAssemblyReference(Assembly assembly)
        {
            Assembly = assembly;
            FileInfo = new FileInfo(assembly.Location);
        }

        public PackageAssemblyReference(Assembly assembly, FileInfo fileInfo)
        {
            Assembly = assembly;
            FileInfo = fileInfo;
        }

        public readonly FileInfo FileInfo;
        public readonly Assembly Assembly;
    }
}
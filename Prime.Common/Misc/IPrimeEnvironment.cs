using System.IO;

namespace Prime.Common
{
    public interface IPrimeEnvironment
    {
        DirectoryInfo StorageDirectory { get; }
    }
}
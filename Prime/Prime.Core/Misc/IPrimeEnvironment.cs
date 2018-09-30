using System.IO;

namespace Prime.Core
{
    public interface IPrimeEnvironment
    {
        DirectoryInfo StorageDirectory { get; }
    }
}
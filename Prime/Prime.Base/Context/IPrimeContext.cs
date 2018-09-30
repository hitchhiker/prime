using System.Collections.Generic;
using System.IO;

namespace Prime.Core
{
    public interface IPrimeContext
    {
        DirectoryInfo AppDataDirectoryInfo { get; }
        ILogger L { get; set; }
    }
}
using System.IO;
using Prime.Base;

namespace Prime.Core
{
    public interface IDataContext
    {
        ObjectId Id { get; }

        bool IsPublic { get; }

        DirectoryInfo StorageDirectory { get; }
    }
}
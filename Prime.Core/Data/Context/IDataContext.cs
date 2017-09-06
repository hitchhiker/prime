using System.IO;
using LiteDB;

namespace Prime.Core
{
    public interface IDataContext
    {
        ObjectId Id { get; }

        bool IsPublic { get; }

        DirectoryInfo StorageDirectory { get; }
    }
}
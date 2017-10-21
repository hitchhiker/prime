using System.IO;
using LiteDB;

namespace Prime.Common
{
    public interface IDataContext
    {
        ObjectId Id { get; }

        bool IsPublic { get; }

        DirectoryInfo StorageDirectory { get; }
    }
}
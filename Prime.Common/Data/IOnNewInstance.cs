using LiteDB;
using Prime.Common;

namespace Prime.Common
{
    public interface IOnNewInstance : IModelBase
    {
        void AfterCreation(IDataContext context, IUniqueIdentifier<ObjectId> parentObject);
    }
}
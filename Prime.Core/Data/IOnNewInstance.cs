using LiteDB;
using Prime.Utility;

namespace Prime.Core
{
    public interface IOnNewInstance : IModelBase
    {
        void AfterCreation(IDataContext context, IUniqueIdentifier<ObjectId> parentObject);
    }
}
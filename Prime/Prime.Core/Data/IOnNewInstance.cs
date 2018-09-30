using LiteDB;
using Prime.Base;
using Prime.Core;

namespace Prime.Core
{
    public interface IOnNewInstance : IModelBase
    {
        void AfterCreation(IDataContext context, IUniqueIdentifier<ObjectId> parentObject);
    }
}
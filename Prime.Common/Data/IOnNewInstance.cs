using LiteDB;
using Prime.Utility;

namespace Prime.Common
{
    public interface IOnNewInstance : IModelBase
    {
        void AfterCreation(IDataContext context, IUniqueIdentifier<ObjectId> parentObject);
    }
}
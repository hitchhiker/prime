using LiteDB;
using Prime.Utility;

namespace Prime.Common
{
    public interface IEmit : IModelBase
    {
        void Emit(IDataContext context, IUniqueIdentifier<ObjectId> parentObject);
    }
}
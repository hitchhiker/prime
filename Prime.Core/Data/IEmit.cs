using LiteDB;
using Prime.Utility;

namespace Prime.Core
{
    public interface IEmit : IModelBase
    {
        void Emit(IDataContext context, IUniqueIdentifier<ObjectId> parentObject);
    }
}
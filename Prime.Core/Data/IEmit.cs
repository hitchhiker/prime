using LiteDB;
using Prime.Core;

namespace Prime.Core
{
    public interface IEmit : IModelBase
    {
        void Emit(IDataContext context, IUniqueIdentifier<ObjectId> parentObject);
    }
}
using LiteDB;
using Prime.Common;

namespace Prime.Common
{
    public interface IEmit : IModelBase
    {
        void Emit(IDataContext context, IUniqueIdentifier<ObjectId> parentObject);
    }
}
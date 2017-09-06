using System.Collections.Concurrent;
using System.Collections.Generic;
using LiteDB;
using Prime.Utility;

namespace Prime.Core
{
    public class AssociatedDatasBase<T> where T : IModelBase, new()
    {
        private readonly ConcurrentDictionary<ObjectId, T> _cache = new ConcurrentDictionary<ObjectId, T>();

        public void CreateAll(IDataContext context, IEnumerable<IUniqueIdentifier<ObjectId>> objects)
        {
            foreach (var i in objects)
                   GetOrCreate(context, i);
        }

        public T GetOrCreate(IDataContext context, IUniqueIdentifier<ObjectId> forObject = null)
        {
            var id = forObject?.Id ?? "prime:associated-single-doc".GetObjectIdHashCode();

            return _cache.GetOrAdd(id, (k) =>
            {
                var d = context.As<T>().FirstOrDefault(x => x.Id == k);
                if (d != null)
                    return Emit(d, context, forObject);

                d = new T { Id = k };

                (d as IOnNewInstance)?.AfterCreation(context, forObject);

                Emit(d, context, forObject);
                d.Save(context);
                return d;
            });
        }

        private static T Emit(T d, IDataContext context, IUniqueIdentifier<ObjectId> forObject)
        {
            if (d is IHasContext)
                (d as IHasContext).Context = context;

            if (d is IEmit)
                (d as IEmit).Emit(context, forObject);
            return d;
        }
    }
}
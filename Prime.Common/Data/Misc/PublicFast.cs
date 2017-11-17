using System;
using LiteDB;

namespace Prime.Common
{
    public class PublicFast
    {
        public static T GetCreate<T>(ObjectId id, Func<T> create, Func<T,bool> confirm = null) where T : IModelBase
        {
            var e = PublicContext.I.As<T>().FirstOrDefault(x => x.Id == id);
            if (e != null && confirm?.Invoke(e)!=false)
                return e;

            e = create();

            if (e == null)
                return e;

            e.SavePublic();
            return e;
        }
    }
}
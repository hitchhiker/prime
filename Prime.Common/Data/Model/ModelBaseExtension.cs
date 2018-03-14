using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using LiteDB;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Prime.Common
{
    public static class ModelBaseExtension
    {
        public static OpResult SavePublic<T>(this T doc) where T : IModelBase
        {
            return Save(doc, PublicContext.I);
        }

        public static OpResult DeletePublic<T>(this T doc) where T : IModelBase
        {
            return Delete(doc, PublicContext.I);
        }

        public static OpResult Save<T>(this T doc, IDataContext context) where T : IModelBase
        {
            return OpResult.From(context.GetDb().Upsert(doc));
        }

        // TODO: AY: Frank review this please.
        [Obsolete("Does not support deleted Keys saving.")]
        public static OpResult SaveAll<T>(this IEnumerable<T> docs, IDataContext context) where T : IModelBase
        {
            return OpResult.From(context.GetDb().Upsert(docs));
        }

        // TODO: AY: Frank review this please.
        public static OpResult SaveAllOverwrite<T>(this IEnumerable<T> docs, IDataContext context) where T : IModelBase
        {
            var db = context.GetDb();
            var deleted = db.Delete<T>(x => true);
            return OpResult.From(db.Upsert(docs));
        }

        public static OpResult Delete<T>(this T doc, IDataContext context) where T : IModelBase
        {
            return OpResult.From(context.GetDb().Delete<T>(doc.Id));
        }

        public static T FirstOrDefault<T>(this LiteQueryable<T> query,  Expression<Func<T, bool>> predicate)
        {
            return query.Where(predicate).FirstOrDefault();
        }

        public static T CloneBson<T>(this T model)
        {
            var doc = BsonMapper.Global.ToDocument(model.GetType(), model);
            return BsonMapper.Global.ToObject<T>(doc);
        }
    }
}
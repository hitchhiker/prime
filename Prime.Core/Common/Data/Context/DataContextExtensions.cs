using System;
using System.Linq;
using Prime.Core;
using LiteDB;

namespace Prime.Core
{
    public static class DataContextExtensions
    {
        public static LiteQueryable<T> As<T>(this IDataContext ctx)
        {
            return GetDb(ctx).Query<T>();
        }

        public static LiteRepository GetDb(this IDataContext ctx)
        {
            return Data.I.GetRepository(ctx);
        }

        public static LiteCollection<T> GetCollection<T>(this IDataContext ctx)
        {
            return Data.I.GetRepository(ctx).Database.GetCollection<T>();
        }

        public static IDataContext OrPublic(this IDataContext ctx)
        {
            return ctx ?? PublicContext.I;
        }

        public static LiteStorage FileDb(this IDataContext ctx)
        {
            return GetDb(ctx).FileStorage;
        }
    }
}
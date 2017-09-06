using LiteDB;

namespace core
{
    public static class PublicContextExtensions
    {
        public static LiteQueryable<T> As<T>(this PublicContext ctx)
        {
            return GetDb(ctx).Query<T>();
        }

        public static LiteRepository GetDb(this PublicContext ctx)
        {
            return Data.I.GetRepository(ctx);
        }

        public static LiteStorage FileDb(this PublicContext ctx)
        {
            return GetDb(ctx).FileStorage;
        }
    }
}
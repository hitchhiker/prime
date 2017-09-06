using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using Prime.Core;
using LiteDB;
using Prime.Utility;

namespace Prime.Core
{
    public class Data : IDisposable
    {
        private Data()
        {
            var g = BsonMapper.Global;

            g.UseCamelCase();
            g.IncludeNonPublic = true;

            g.ResolveMember = ResolveMember;

            g.RegisterType<Asset>
            (
                obj => obj.ShortCode,
                bson => Assets.I.GetRaw(bson.AsString)
            );
            g.RegisterType<AssetPair>
            (
                obj => obj==null ? "" : obj.Asset1?.ShortCode + ":" + obj.Asset2?.ShortCode,
                delegate(BsonValue bson)
                {
                    var p = bson.AsString.Split(':');
                    return p.Length == 2 ? new AssetPair(Assets.I.GetRaw(p[0]), Assets.I.GetRaw(p[1])) : null;
                }
            );

            g.RegisterType<INetworkProvider>(
                i => i.Id,
                value => Networks.I.Providers.FirstOrDefault(x => x.Id == value.AsObjectId)
            );

            g.RegisterType<Network>(
                i => i.Id,
                value => Networks.I.FirstOrDefault(x=>x.Id == value.AsObjectId)
            );

            //LiteDB had to be patched (removed its native datetime/bson mapper to allow RegisterType<T>).
            g.RegisterType<DateTime>(BsonMapperMods.DateTimeSerialize, BsonMapperMods.DateTimeDeserialize);
        }

        private void ResolveMember(Type type, MemberInfo memberInfo, MemberMapper arg3)
        {
            if (arg3.FieldName == "_id")
                return;

            var isbson = memberInfo.CustomAttributes.Any(x => x.AttributeType.GetTypeInfo().Name.Contains("bson", StringComparison.OrdinalIgnoreCase));
            if (isbson)
                return;

            arg3.FieldName = null; //remove this entry
        }

        public static Data I => Lazy.Value;
        private static readonly Lazy<Data> Lazy = new Lazy<Data>(()=>new Data());

        /// <summary>
        /// Must be disposed properly
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public LiteRepository GetRepository(IDataContext ctx)
        {
            var dbname = ctx.IsPublic ? "pub" : ctx.Id.ToString();
            return _cache.GetOrAdd(ctx.Id, k =>
            {
                var loc = Path.Combine(ctx.StorageDirectory.FullName, "db");
                loc = Path.Combine(loc, dbname + ".db");
                var fi = new FileInfo(loc);
                if (!Directory.Exists(fi.Directory.FullName))
                    Directory.CreateDirectory(fi.Directory.FullName);

                return new LiteRepository(loc);
            });
        }

        private readonly ConcurrentDictionary<ObjectId, LiteRepository> _cache = new ConcurrentDictionary<ObjectId, LiteRepository>();

        public static object DataLock = new object();

        public T GetOrCreateIdModel<T>(LiteQueryable<T> col, ObjectId id, Action<T> save) where T : ModelBase, new()
        {
            lock (DataLock)
            {
                var d = col.FirstOrDefault(x => x.Id == id);
                if (d != null)
                    return d;

                d = new T {Id = id};
                save.Invoke(d);
                return d;
            }
        }

        public T GetOrCreateIdPublicModel<T>(ObjectId id) where T : ModelBase, new()
        {
            return GetOrCreateIdModel(PublicContext.I.As<T>(), id, obj => obj.SavePublic() );
        }

        public T GetOrCreateIdUserModel<T>(UserContext context, ObjectId id) where T : ModelBase, new()
        {
            return GetOrCreateIdModel(context.As<T>(), id, obj => obj.Save(context));
        }

        public void Dispose()
        {
            _cache.Values.ForEach(x => x.Dispose());
        }
    }
}
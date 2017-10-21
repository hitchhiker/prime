using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

namespace Prime.Common
{
    public static class BsonMapperMods
    {
        public static DateTime DateTimeDeserialize(BsonValue bson)
        {
            var dt = bson.AsInt64;

            if (dt== long.MaxValue)
                return DateTime.MaxValue;
           
            if (dt==0)
                return DateTime.MinValue;

            return DateTime.SpecifyKind(new DateTime(bson.AsInt64), DateTimeKind.Utc);
        }

        public static BsonValue DateTimeSerialize(DateTime obj)
        {
            if (obj == DateTime.MinValue)
                return 0;

            if (obj == DateTime.MaxValue)
                return long.MaxValue;

            if (obj.Kind != DateTimeKind.Utc)
                throw new Exception("Only UTC DateTime values are serialisable.");

            return DateTime.SpecifyKind(obj, DateTimeKind.Local).Ticks;
        }
    }
}

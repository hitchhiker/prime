using System;
using LiteDB;
using Prime.Common;

namespace Prime.Common
{
    public class ProviderData : ModelBase
    {
        public static object Lock = new Object();
    }
}
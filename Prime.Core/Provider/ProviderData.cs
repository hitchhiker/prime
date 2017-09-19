using System;
using LiteDB;
using Prime.Utility;

namespace Prime.Core
{
    public class ProviderData : ModelBase
    {
        public static object Lock = new Object();
    }
}
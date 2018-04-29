using System;
using LiteDB;
using Prime.Core;

namespace Prime.Core
{
    public class ProviderData : ModelBase
    {
        public static object Lock = new Object();
    }
}
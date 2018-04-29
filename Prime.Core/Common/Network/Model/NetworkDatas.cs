using System;

namespace Prime.Core
{
    public class NetworkDatas : AssociatedDatasBase<NetworkData>
    {
        private NetworkDatas() {}

        public static NetworkDatas I => Lazy.Value;
        private static readonly Lazy<NetworkDatas> Lazy = new Lazy<NetworkDatas>(()=>new NetworkDatas());
    }
}
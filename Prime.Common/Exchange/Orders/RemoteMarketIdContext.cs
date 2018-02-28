using System;
using Prime.Utility;

namespace Prime.Common
{
    /// <summary>
    /// This context is used to get order information in case when except id the market where the order was placed is required.
    /// </summary>
    public class RemoteMarketIdContext : RemoteIdContext
    {
        public RemoteMarketIdContext(UserContext userContext, string remoteGroupId, AssetPair market = null, ILogger logger = null) : base(userContext, remoteGroupId, logger)
        {
            Market = market;
        }

        /// <summary>
        /// Market where order was placed.
        /// </summary>
        public readonly AssetPair Market;
    }
}
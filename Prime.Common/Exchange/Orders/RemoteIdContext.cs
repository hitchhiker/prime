using System;
using System.Collections.Generic;
using System.Text;
using Prime.Utility;

namespace Prime.Common
{
    /// <summary>
    /// This context is used to get details of order, withdrawal, deposit and other actions that have unique Id.
    /// </summary>
    public class RemoteIdContext : NetworkProviderPrivateContext
    {
        public readonly string RemoteGroupId;

        public RemoteIdContext(UserContext userContext, string remoteId, ILogger logger = null) : base(userContext, logger)
        {
            RemoteGroupId = remoteId;
        }
    }
}

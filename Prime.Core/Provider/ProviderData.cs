using System;
using LiteDB;
using Prime.Utility;

namespace Prime.Core
{
    public class ProviderData : ModelBase, IKeepFresh, IOnNewInstance
    {
        public static object Lock = new Object();

        [Bson]
        public ApiKeys ApiKeys { get; private set; } = new ApiKeys();

        public void Refresh(IDataContext ctx, INetworkProvider provider, bool save = true)
        {
            if (ApiKeys.FsDone)
                return;

            ApiKeys.CollectFilesystem(provider);
            if (save)
                this.Save(ctx);
        }

        public void AfterCreation(IDataContext context, IUniqueIdentifier<ObjectId> parentObject)
        {
            Refresh(context, parentObject as INetworkProvider, false);
        }
    }
}
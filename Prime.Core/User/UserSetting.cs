using System;
using LiteDB;
using Prime.Utility;
using System.Collections.Generic;

namespace Prime.Core
{
    public class UserSetting : ModelBase, IOnNewInstance
    {
        public static object Lock = new Object();

        [Bson]
        public BookmarkedCommands BookmarkedCommands { get; private set; } = new BookmarkedCommands();

        [Bson]
        public UniqueList<WalletAddress> Addresses { get; private set; } = new UniqueList<WalletAddress>();

        public void AfterCreation(IDataContext context, IUniqueIdentifier<ObjectId> parentObject)
        {
            BookmarkedCommands.Defaults();
        }
    }
}
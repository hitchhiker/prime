using System;
using LiteDB;
using Prime.Utility;

namespace Prime.Core
{
    public class UserSetting : ModelBase, IOnNewInstance
    {
        public static object Lock = new Object();

        [Bson]
        public BookmarkedCommands Bookmarks { get; private set; } = new BookmarkedCommands();

        [Bson]
        public UniqueList<WalletAddress> Addresses { get; private set; } = new UniqueList<WalletAddress>();

        [Bson]
        public UniqueList<AssetPair> FavouritePairs { get; set; } = new UniqueList<AssetPair>();

        [Bson]
        public UniqueList<AssetPair> HistoricExchangeRates { get; set; } = new UniqueList<AssetPair>();

        public void AfterCreation(IDataContext context, IUniqueIdentifier<ObjectId> parentObject)
        {
            Bookmarks.Defaults();
        }
    }
}
using System;
using LiteDB;
using Prime.Base;
using Prime.Core;

namespace Prime.Core
{
    public class UserSetting : ModelBase, IOnNewInstance
    {
        public static object Lock = new Object();

        [Bson]
        public BookmarkedCommands Bookmarks { get; private set; } = new BookmarkedCommands();

        public void AfterCreation(IDataContext context, IUniqueIdentifier<ObjectId> parentObject)
        {
            Bookmarks.Defaults();
        }
    }
}
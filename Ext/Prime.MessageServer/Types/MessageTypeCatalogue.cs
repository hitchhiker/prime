using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Prime.Core
{
    /// <summary>
    /// Manages a collection of all system types and their hashes in 2 seperate *oppositely indexed* dictionaries for extremely fast lookup.
    /// </summary>
    public sealed class MessageTypeCatalogue : TypeIndexDictionariesBase<string>
    {
        private readonly ExtensionManager _manager;
        public static object Lock = new object();
        public static bool FilterTypeCatalogueAttribute = false;

        public MessageTypeCatalogue(ExtensionManager manager) : base(Results(manager), GetHash)
        {
            _manager = manager;
        }

        private static string GetHash(Type type)
        {
            return type.FullName.ToLower().Replace(".messages","");
        }

        private static IEnumerable<Type> Results(ExtensionManager manager)
        {
            return manager.Types.Implements<BaseTransportMessage>();
        }
    }
}
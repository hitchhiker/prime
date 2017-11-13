using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;

namespace Prime.Utility
{
    /// <summary>
    /// Manages a collection of all system types and their hashes in 2 seperate *oppositely indexed* dictionaries for extremely fast lookup.
    /// </summary>
    public sealed class TypeCatalogue : TypeIndexDictionariesBase<int>
    {
        public static object Lock = new object();

        private TypeCatalogue() : base(Results(), ReflectionExtensionMethods.GetHashShortName) {}

        public static TypeCatalogue I { get { return Lazy.Value; } }
        private static readonly Lazy<TypeCatalogue> Lazy = new Lazy<TypeCatalogue>(() => new TypeCatalogue());

        private static IEnumerable<Type> Results()
        {
            var result = new List<Type>();
            var t = typeof(IncludeInTypeCatalogueAttribute);

            foreach (var a in AssemblyCatalogue.I.Assemblies)
            {
                if (a.CustomAttributes.All(x => x.AttributeType != t))
                    continue;

                try
                {
                    result.AddRange(a.GetTypes());
                }
                catch (ReflectionTypeLoadException tle)
                {
                    var msgs = tle.LoaderExceptions.Select(x => x.Message).ToList();
                    msgs.Add("In " + a.GetName());
                    throw new Exception(string.Join(", ", msgs) +
                                        " This is normally due to an installation issue or missing extension.");
                }
                catch (Exception e)
                {
                    throw new Exception("Problem loading types: " + e);
                }
            }
            return result;
        }

        public Type Get(int? hash)
        {
            return hash == null ? null : base.Get((int)hash);
        }
    }
}
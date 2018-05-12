using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Prime.Core
{
    /// <summary>
    /// Manages a collection of all non-GAC assemblies hashes in 2 seperate *oppositely indexed* dictionaries for extremely fast lookup.
    /// </summary>
    public sealed class AssemblyCatalogue : IEnumerable<Assembly>
    {
        public static object Lock = new object();

        private readonly Dictionary<Assembly, int> _hashLookup;
        private readonly Dictionary<int, Assembly> _typeLookup;
        private readonly List<Assembly> _assemblies;
        public IReadOnlyList<Assembly> Assemblies => _assemblies;

        public AssemblyCatalogue()
        {
            _assemblies = Results();
            _hashLookup = _assemblies.ToDictionary(x => x, y => y.GetHashCode());
            try
            {
                _typeLookup = _hashLookup.ToDictionary(x => x.Value, y => y.Key);
            }
            catch (Exception e)
            {
                var report = "Key collision: " + string.Join(Environment.NewLine, _hashLookup.ToList().GroupBy(x => x.Value).Where(x => x.Count() > 1).Select(x => x.Key + ": " + string.Join(", ", x)));
                throw new Exception(report, e);
            }
        }

        private List<Assembly> Results()
        {
            var result = new List<Assembly>();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var a in assemblies)
            {
                try
                {
                    if (a.IsDynamic || a.GlobalAssemblyCache)
                        continue;

                    if (a.FullName.StartsWith("Microsoft.", StringComparison.OrdinalIgnoreCase) || a.FullName.StartsWith("System.", StringComparison.OrdinalIgnoreCase) || a.FullName.StartsWith("NETStandard", StringComparison.OrdinalIgnoreCase))
                        continue;

                    if (a.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright?.Contains("microsoft", StringComparison.OrdinalIgnoreCase) == true)
                        continue;
                }
                catch
                {
                    continue;
                }

                try
                {
                    result.Add(a);
                }
                catch (ReflectionTypeLoadException tle)
                {
                    throw new Exception(string.Join(", ", tle.LoaderExceptions.Select(x => x.Message)) + " This is normally due to an installation issue or missing extension.");
                }
                catch (Exception e)
                {
                    throw new Exception("Problem loading assemblies: " + e);
                }
            }
            return result;
        }

        public int Get(Assembly assembly)
        {
            return _hashLookup.Get(assembly);
        }

        public Assembly Get(int hash)
        {
            return _typeLookup.Get(hash, null);
        }

        public IEnumerator<Assembly> GetEnumerator()
        {
            return _assemblies.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return string.Join(", ", this.Select(x => x.GetShortName()));
        }
    }
}
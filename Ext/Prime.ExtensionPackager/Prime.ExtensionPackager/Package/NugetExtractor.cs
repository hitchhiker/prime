using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prime.Core;

namespace Prime.ExtensionPackager
{
    public class NugetExtractor
    {
        private readonly ProgramContext _context;
        private readonly Package _package;

        public NugetExtractor(ProgramContext context, Package package)
        {
            _context = context;
            _package = package;
        }

        public void Extract(DirectoryInfo path)
        {
            var depsPath = _package.Assembly.Location.Replace(".dll", ".deps.json").Replace(".exe", ".deps.json");
            var depsInfo = new FileInfo(depsPath);
            if (!depsInfo.Exists)
            {
                Console.WriteLine("No .deps.json file found.");
                return;
            }

            var paths = new List<string>();

            if (JsonConvert.DeserializeObject(File.ReadAllText(depsInfo.FullName)) is JObject deps)
                Extract(path, deps, paths);
            else
                Console.WriteLine("Unable to parse .deps.json");

            if (paths.Count > 0)
                ExtractPaths(path, paths);
        }

        private void Extract(DirectoryInfo path, JObject deps, List<string> paths)
        {
            var targets = deps["targets"];
            foreach (var t in targets.Children().OfType<JProperty>())
            {
                if (!t.Value.HasValues)
                    continue;

                var items = t.Value.Children();
                foreach (var i in items.OfType<JProperty>().Select(x=>x.Value).Children().OfType<JProperty>())
                {
                    if (!i.Value.HasValues || i.Name!="runtime")
                        continue;

                    var v = i.Value.Children();
                    var prop = v.First() as JProperty;
                    paths.Add((i.Parent.Parent as JProperty).Name + Path.DirectorySeparatorChar + prop.Name);
                }
            }
        }

        private void ExtractPaths(DirectoryInfo path, List<string> paths)
        {
            var nuget = new DirectoryInfo(_context.C.Config.NugetPath.ResolveSpecial());
            if (!nuget.Exists)
                throw new Exception(".nuget folder not found at: " + nuget.FullName);

            var nugetPackages = Path.Combine(nuget.FullName, "packages");

            foreach (var p in paths)
            {
                var fi = new FileInfo(Path.Combine(nugetPackages, p));
                if (fi.Exists)
                    File.Copy(fi.FullName, Path.Combine(path.FullName, fi.Name), true);
            }
        }
    }
}
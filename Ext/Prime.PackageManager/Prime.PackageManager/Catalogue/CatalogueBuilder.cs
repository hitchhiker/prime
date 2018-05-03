using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Prime.PackageManager
{
    public class CatalogueBuilder
    {
        private readonly PackageManagerContext _context;

        public CatalogueBuilder(PackageManagerContext context)
        {
            _context = context;
        }

        public void Build()
        {
            var dirs = _context.DistributionDirectory.EnumerateDirectories().ToList();

            _context.Logger.Info("Scanning distribution directory: " + _context.DistributionDirectory.FullName);

            if (dirs.Count == 0)
            {
                _context.Logger.Warn("Distribution directory is empty.");
                return;
            }

            var cat = Discover(dirs);

            _context.Logger.Info($"{cat.Count} entries found.");

            var jsonObject = CreateJsonObject(cat);

            var catJson = new FileInfo(Path.Combine(_context.CatalogueDirectory.FullName, "cat.json"));

            if (catJson.Exists)
                catJson.Delete();

            File.WriteAllText(catJson.FullName, JsonConvert.SerializeObject(jsonObject));
        }

        private Catalogue Discover(List<DirectoryInfo> dirs)
        {
            var cat = new Catalogue();
            foreach (var d in dirs)
            {
                foreach (var sd in d.GetDirectories())
                {
                    try
                    {
                        var entry = CatalogueEntry.Rebuild(sd);
                        if (entry != null)
                            cat.Add(entry);
                    }
                    catch (Exception e)
                    {
                        _context.Logger.Error(e.Message + " in " + sd.FullName);
                    }
                }
            }

            return cat;
        }

        private CatalogueSchema CreateJsonObject(Catalogue catalogue)
        {
            var cs = new CatalogueSchema {Owner = "prime"};

            foreach (var group in catalogue.GroupBy(x=>x.MetaData.Id))
            {
                var ces = new CatalogueEntrySchema() { Id = group.Key.ToString() };
                cs.Catalogue.Add(ces);

                foreach (var i in group)
                {
                    ces.Entries.Add(new CatalogueItemSchema() {Version = i.MetaData.Version.ToString(), Platform = i.MetaData.Platform.ToString()});
                }
            }

            return cs;
        }
    }
}
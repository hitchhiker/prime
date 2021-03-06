﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Prime.Base;
using Prime.Core;

namespace Prime.Radiant
{
    public class CatalogueHelper
    {
        public static string IndexName = "index.json";
        public static string IndexArchiveName = "index.arc";
        public static string CatName = "cat.json";
        public static string CatArchiveName = "cat.arc";

        public static CatalogueIndex ExtractIndex(PrimeContext context, CataloguePublisherConfig config)
        {
            return ExtractIndex(context, config.GetIndexArchiveInfo(context));
        }

        public static CatalogueIndex ExtractIndex(PrimeContext context, FileInfo indexArcFi)
        {
            if (!indexArcFi.Exists)
                return new CatalogueIndex() { CurrentRevision = 0, UtcCreated = DateTime.UtcNow };

            var tmpDir = indexArcFi.ExtractArchive(context);

            var indexFi = new FileInfo(Path.Combine(tmpDir.FullName, IndexName));


            var index = !indexFi.Exists ?
                new CatalogueIndex() { CurrentRevision = 0, UtcCreated = DateTime.UtcNow } :
                JsonConvert.DeserializeObject<CatalogueIndex>(File.ReadAllText(indexFi.FullName));

            if (indexFi.Exists && !indexFi.VerifySignedFile(index.PublicKey))
            {
                context.L.Warn($"{indexFi.FullName} failed signature verification as is being ignored.");
                index = new CatalogueIndex() {CurrentRevision = 0, UtcCreated = DateTime.UtcNow};
            }

            tmpDir.Delete(true);

            return index;
        }

        public static CatalogueIndex ExtractIndexFromCatalogueDirectory(PrimeContext context, DirectoryInfo catDir)
        {
            var indexArcFi = new FileInfo(Path.Combine(catDir.FullName, IndexArchiveName));
            return ExtractIndex(context, indexArcFi);
        }

        public static ICatalogue ExtractCatalogue(PrimeContext context, FileInfo catArchiveFi, Type catalogueType)
        {
            var tmpDir = catArchiveFi.ExtractArchive(context);
            var extr = tmpDir.EnumerateFiles().ToList();

            var catFi = extr.FirstOrDefault(x => x.Name.EndsWith(".json"));
            var signFi = extr.FirstOrDefault(x => x.Name.EndsWith(".sign"));

            if (!(JsonConvert.DeserializeObject(File.ReadAllText(catFi.FullName), catalogueType) is ICatalogue catalogue))
            {
                context.L.Fatal("Retreived the catalogue file from IPFS, but was unable to deserialise it. Aborting.");
                return null;
            }

            if (string.IsNullOrWhiteSpace(catalogue.PublicKey))
            {
                context.L.Fatal("Retreived the catalogue but it does not contain a public key. Aborting.");
                return null;
            }

            if (!catFi.VerifySignedFile(catalogue.PublicKey.ToPublicKey()))
            {
                context.L.Fatal("Retreived the catalogue but it failed signature verification. Aborting.");
                return null;
            }

            tmpDir.Delete(true);

            return catalogue;
        }
    }
}

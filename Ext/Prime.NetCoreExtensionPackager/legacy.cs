using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using CommandLine;
using Newtonsoft.Json;
using Prime.Core;

namespace Prime.NetCoreExtensionPackager
{
    public class legacy
    {
        public static void Run(NetCoreExtensionPackagerArguments opts)
        {
            NetCoreExtensionPackagerExtension.DummyRef();

            var extPath = "./";
            var configPath = "./";

            var logger = new ConsoleLogger() {IncludePreamble = false};

            configPath = Path.GetFullPath(opts.ConfigPath);
            extPath = Path.GetFullPath(opts.ExtPath);

            var extDir = new DirectoryInfo(extPath);
            if (!extDir.Exists)
            {
                logger.Info("No such directory: " + extPath);
                return;
            }

            var pc = new PrimeContext(configPath);

            logger.Info("");
            logger.Info(" Extension: " + extPath);
            logger.Info(" Config: " + configPath);

            var ctx = new NetCorePackagerContext(pc)
            {
                SourceDirectory = extDir,
                //IsBase = opts.ExtensionKey=="prime:base"
            };

            if (opts.ExtensionKey != null)
                ctx.ExtId = opts.ExtensionKey.GetObjectIdHashCode();

            if (opts.ExtensionId != null)
                ctx.ExtId = opts.ExtensionId;

            ctx.ExtractNuget = opts.Nuget;

            NetCorePackager.PackageItem(ctx);

            logger.Info("");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using CommandLine;
using Newtonsoft.Json;
using Prime.Base;
using Prime.Core;

namespace Prime.ExtensionPackager
{
    public class Program
    {
        class Options
        {
            [Option('c', "config", Required = true, HelpText = "Path to the 'prime-client.config' file.")]
            public string ConfigPath { get; set; }

            [Option('e', "ext", Required = true, HelpText = "Path to the extension directory for inspection.")]
            public string ExtPath { get; set; }

            [Option("key", HelpText = "Used to force inspection of only the extension matching this hashed name key.")]
            public string ExtensionKey { get; set; }

            [Option("id", HelpText = "Used to force inspection of only the extension matching this id.")]
            public ObjectId ExtensionId { get; set; }

            [Option("nuget", HelpText = "Used to force collection of missing nuget libraries [experimental].")]
            public bool Nuget { get; set; }

            // Omitting long name, defaults to name of property, ie "--verbose"
            [Option(Default = false, HelpText = "Prints all messages to standard output.")]
            public bool Verbose { get; set; }
        }

        static void Main(string[] args)
        {
            if (Debugger.IsAttached)
            {
                Run(new Options()
                {
                    ConfigPath = "..\\..\\..\\..\\..\\..\\instance\\prime-client.config",
                    //ExtPath = "..\\..\\..\\..\\..\\..\\Ext\\Prime.IPFS\\Prime.IPFS.Win64\\bin\\Debug"
                    ExtPath = "..\\..\\..\\..\\..\\..\\Ext\\Prime.Finance\\bin\\Debug\\netstandard2.0"
                    //ExtPath = "..\\..\\..\\..\\..\\..\\Ext\\Prime.KeysManager\\bin\\Debug\\netcoreapp2.0",
                    //ExtensionKey = "prime:KeysManagerExtension"
                });
                Console.ReadLine();
            } else
                CommandLine.Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(Run);        }

        static void Run(Options opts)
        {
            ExtensionInfo.DummyRef();

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

            var ctx = new ProgramContext(pc)
            {
                SourceDirectory = extDir,
                IsBase = opts.ExtensionKey=="prime:base",
                Logger = logger
            };

            if (opts.ExtensionKey != null)
                ctx.ExtId = opts.ExtensionKey.GetObjectIdHashCode();

            if (opts.ExtensionId != null)
                ctx.ExtId = opts.ExtensionId;

            ctx.ExtractNuget = opts.Nuget;

            Process.Go(ctx);

            logger.Info("");
        }
    }
}

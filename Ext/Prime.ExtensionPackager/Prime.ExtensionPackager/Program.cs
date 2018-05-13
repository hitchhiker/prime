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

            [Option("core", HelpText = "Set to true if inspecting the prime core project")]
            public bool IsCore { get; set; }

            [Option("key", HelpText = "Used to force inspection of only the extension matching this hashed name key.")]
            public string ExtensionKey { get; set; }

            [Option("id", HelpText = "Used to force inspection of only the extension matching this id.")]
            public ObjectId ExtensionId { get; set; }

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
                    ExtPath = "..\\..\\..\\..\\..\\..\\Ext\\Prime.KeysManager\\bin\\Debug\\netcoreapp2.0",
                    ExtensionKey = "prime:KeysManagerExtension"
                });
            } else
                CommandLine.Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(Run);
        }

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

            var pc = new ClientContext(configPath);

            logger.Info("config: " + configPath);
            logger.Info("extension: " + extPath);

            var ctx = new ProgramContext(pc)
            {
                SourceDirectory = extDir,
                IsPrime = opts.IsCore,
                Logger = logger
            };

            if (opts.ExtensionKey != null)
                ctx.ExtId = opts.ExtensionKey.GetObjectIdHashCode();

            if (opts.ExtensionId != null)
                ctx.ExtId = opts.ExtensionId;

            Process.Go(ctx);

            logger.Info("");
        }
    }
}

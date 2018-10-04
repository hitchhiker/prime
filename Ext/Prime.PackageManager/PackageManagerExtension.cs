using System;
using System.Collections.Generic;
using System.Composition;
using System.Text;
using CommandLine;
using Newtonsoft.Json;
using Prime.Base;
using Prime.Base.Messaging.Common;
using Prime.Core;
using Prime.PackageManager;
using Prime.PackageManager.Messages;
using Prime.Radiant;

namespace Prime.Core
{
    [Export(typeof(IExtension))]
    public class PackageManagerExtension : IExtensionInitPrimeInstance, IExtensionInstanceCommandArgs
    {
        private static readonly ObjectId _id = "prime:pm".GetObjectIdHashCode();
        public ObjectId Id => _id;
        public string Title => "Prime Package Manager";
        public Version Version => new Version("1.0.4");

        public void InitialiseCommandArgs(PrimeInstance instance, CommandArgs argsParser)
        {
            argsParser.Add(new CommandArg("package", "Packages management.", args =>
            {
                Parser.Default.ParseArguments<PackageManagerArguments.BuildArguments,
                        PackageManagerArguments.CompileArguments,
                        PackageManagerArguments.BundleArguments,
                        PackageManagerArguments.PublishArguments,
                        PackageManagerArguments.UpgradeArguments>(args)
                        .WithParsed<PackageManagerArguments.PublishArguments>(o =>PackageManagerEntry.RequestPublish(instance, o))
                    .WithParsed<PackageManagerArguments.CompileArguments>(o =>PackageManagerEntry.RequestCompile(instance, o))
                    .WithParsed<PackageManagerArguments.BundleArguments>(o =>PackageManagerEntry.RequestBundle(instance, o))
                    .WithParsed<PackageManagerArguments.BuildArguments>(o =>PackageManagerEntry.RequestBuild(instance, o))
                    .WithParsed<PackageManagerArguments.UpgradeArguments>(o =>PackageManagerEntry.RequestUpgrade(instance, o));
            }));
        }

        public void Init(PrimeInstance instance)
        {
            instance.M.RegisterAsync(this, (PrimePackagesRequest m) =>
            {
                PackageManagerEntry.RequestBuild(instance, new PackageManagerArguments.BuildArguments());
                instance.M.Send(new PrimePackagesResponse(m) {Success = true});
            });
        }

        public static void DummyRef()
        {
            Noop(typeof(JsonReaderException));
        }

        private static void Noop(Type _) { }
    }
}

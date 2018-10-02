using System;
using System.Composition;
using CommandLine;
using Newtonsoft.Json;
using Prime.Base;
using Prime.Core;

namespace Prime.Radiant
{
    [Export(typeof(IExtension))]
    public class RadiantExtension : IExtensionInitPrimeInstance, IExtensionInstanceCommandArgs
    {
        private static readonly ObjectId _id = "prime:radiant".GetObjectIdHashCode();
        public ObjectId Id => _id;
        public string Title => "Prime Radiant Catalogue Manager";
        public Version Version => new Version("1.0.4");
        public RadiantMessenger Messenger { get; private set; }

        public void Init(PrimeInstance instance)
        {
            Messenger = new RadiantMessenger(instance);
        }

        public void InitialiseCommandArgs(PrimeInstance instance, CommandArgs argsParser)
        {
            argsParser.Add(new CommandArg("catalogue", "Catalogue management.", args =>
            {
                Parser.Default.ParseArguments<RadiantArguments.UpdateArguments, RadiantArguments.PublishArguments>(args)
                    .WithParsed<RadiantArguments.UpdateArguments>(o => RadiantEntry.EnterUpdate(instance, o))
                    .WithParsed<RadiantArguments.PublishArguments>(o => RadiantEntry.EnterPublish(instance, o));
            }));
        }

        public static void DummyRef()
        {
            Noop(typeof(JsonReaderException));
        }

        private static void Noop(Type _) { }
    }
}

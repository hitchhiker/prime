using System;
using System.Composition;
using System.Threading;
using CommandLine;
using LiteDB;
using Prime.Base;
using Prime.Base.Messaging.Common;

namespace Prime.Core
{
    [Export(typeof(IExtension))]
    public class PrimeCoreExtension : IPrime, IExtensionInstanceCommandArgs
    {
        private static readonly ObjectId _id = "prime:core".GetObjectIdHashCode();
        public ObjectId Id => _id;

        public string Title { get; } = "Prime";

        public Version Version { get; } = new Version("1.0.5");

        public void InitialiseCommandArgs(PrimeInstance instance, CommandArgs argsParser)
        {
            argsParser.Add(new CommandArg("daemon", "Starts a long running Prime server instance.", args =>
            {
                void ServerWaitLoop(PrimeCoreArguments.Start o)
                {
                    do
                    {
                        Thread.Sleep(1);
                    } while (instance.IsStarted);
                }

                Parser.Default.ParseArguments<PrimeCoreArguments.Start>(args).WithParsed(ServerWaitLoop);
            }));
        }
    }
}
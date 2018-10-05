using System;
using System.Composition;
using Newtonsoft.Json;
using Prime.Base;
using Prime.BootstrapInstallCreator;
using Prime.Core;

namespace Prime.NetCoreExtensionPackager
{
    [Export(typeof(IExtension))]
    public class BootstrapInstallCreatorExtension : IExtension
    {
        private static readonly ObjectId _id = "prime:BootstrapInstallCreator".GetObjectIdHashCode(true,true);
        public ObjectId Id => _id;
        public string Title => "Prime Bootstrap Install Creator";
        public Version Version => new Version("1.0.5");

        public static void DummyRef()
        {
            Noop(typeof(JsonReaderException));
        }

        private static void Noop(Type _) { }

        public void Build(PrimeContext context)
        {
            var builder = new Builder(new BuilderContext(context) {OsKey = "linux-x64", ProjectDir = "[src]/Platform/Linux/Prime.Lin64/", TemplateKey = "lin64"});
            builder.Build();
        }
    }
}

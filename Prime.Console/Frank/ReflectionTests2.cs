using System.IO;
using Prime.Core;
using Prime.Extensions;

namespace Prime.Console.Frank
{
    public partial class ReflectionTests2 : TestServerBase
    {
        public ReflectionTests2(Core.PrimeInstance server) : base(server)
        {
        }

        public override void Go()
        {
            /*
            var path = @"V:\prime\src\instance\prime-server\package\install\prime-ipfs-go-3575ddcb0d8647d75fbf044c\1.3.0-winamd64";
            var dir = new DirectoryInfo(path);

            var loader = new ExtensionLoaderComposition();
            var ext = loader.LoadExtension<IExtension>(dir);*/

            var e = P.ExtensionManager;
            
            foreach (var ext in e.Instances)
                L.Log(ext.Extension.Title + " loaded: " + ext.IsLoaded);
        }
    }
}
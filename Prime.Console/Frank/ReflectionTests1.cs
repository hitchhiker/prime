using Prime.Core;

namespace Prime.Console.Frank
{
    public class ReflectionTests1 : TestServerBase
    {
        public ReflectionTests1(ServerContext server) : base(server)
        {
        }

        public override void Go()
        {
            var prime = new Core.Prime(S);
            var e = prime.Extensions;

            foreach (var a in e.Assemblies.OrderBy(x=>x.FullName, SortingDirection.Ascending))
                L.Log(a.FullName);

            foreach (var a in e.Types.ImplementInstances<IExtension>())
                L.Log(ExtensionLoaderExtensionMethods.ToString(a));
        }
    }
}
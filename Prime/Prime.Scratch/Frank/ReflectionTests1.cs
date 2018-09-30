using Prime.Core;

namespace Prime.Scratch
{
    public class ReflectionTests1 : TestServerBase
    {
        public ReflectionTests1(Core.PrimeInstance server) : base(server)
        {
        }

        public override void Go()
        {
            var prime = P;
            var e = prime.ExtensionManager;

            foreach (var a in e.Assemblies.OrderBy(x=>x.FullName, SortingDirection.Ascending))
                L.Log(a.FullName);

            foreach (var a in e.Types.ImplementInstances<IExtension>())
                L.Log(ExtensionLoaderExtensionMethods.ToString(a));
        }
    }
}
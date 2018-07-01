using Prime.Core;

namespace Prime.Console.Frank
{
    public class ReflectionTests2 : TestServerBase
    {
        public ReflectionTests2(ServerContext server) : base(server)
        {
        }

        public override void Go()
        {
            var prime = new Core.Prime(S);
            var e = prime.Extensions;

            e.LoadInstalled();

            foreach (var ext in e)
                L.Log(ext.Extension.Title);
        }
    }
}
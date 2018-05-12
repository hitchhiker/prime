using System;
using Prime.Core;

namespace Prime.ConsoleApp.Tests.Frank
{
    public class ReflectionTests : TestServerBase
    {
        public ReflectionTests(ServerContext server) : base(server)
        {
        }

        public override void Go()
        {
            var prime = new Core.Prime(S);
            var e = prime.Extensions;

            foreach (var a in e.Assemblies.OrderBy(x=>x.FullName, SortingDirection.Ascending))
                L.Log(a.FullName);
        }
    }
}
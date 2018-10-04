using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using Prime.Core;

namespace Prime.Scratch
{
    public class ReflectionTests3 : TestServerBase
    {
        public ReflectionTests3(PrimeInstance server) : base(server)
        {
        }

        public override void Go()
        {
            var lc = new List<RefEmitLoadContext>();

            var l1 = new RefEmitLoadContext();
            var a = l1.LoadFromAssemblyPath(@"D:\hh\git\prime\src\instance\prime-client\package\install\prime-f7015e4f838b8f7439722bb6\0.3.6850.36466\Prime.Core.dll");
            L.Log(a.FullName);

            var l2 = new RefEmitLoadContext();
            var a2 = l2.LoadFromAssemblyPath(@"D:\hh\git\prime\src\instance\prime-client\package\install\prime-f7015e4f838b8f7439722bb6\0.3.6850.25934\Prime.Core.dll");
            L.Log(a2.FullName);


            //var a = Assembly.LoadFrom("D:\\hh\\git\\prime\\src\\instance\\prime-client\\tmp\\publish\\Prime.Base\\Prime.Base.dll");

        }

        public class RefEmitLoadContext : AssemblyLoadContext
        {
            protected override Assembly Load(AssemblyName assemblyName)
            {
                throw new NotImplementedException();
            }
        }
    }
}
using System.Reflection;
using Prime.Base;
using Prime.Core;

namespace Prime.NetCoreExtensionPackager
{
    public class ReflectionTest : CommonBase
    {
        public ReflectionTest(PrimeContext c) : base(c)
        {
        }

        public void Go()
        {
            var a = Assembly.LoadFrom("D:\\hh\\git\\prime\\src\\instance\\prime-client\\tmp\\publish\\Prime.Base\\Prime.Base.dll");
            L.Log(a.FullName);
        }
    }
}
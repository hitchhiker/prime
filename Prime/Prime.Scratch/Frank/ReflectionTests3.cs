using System.Reflection;
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
            var a = Assembly.LoadFrom("D:\\hh\\git\\prime\\src\\instance\\prime-client\\tmp\\publish\\Prime.Base\\Prime.Base.dll");
            L.Log(a.FullName);
        }
    }
}
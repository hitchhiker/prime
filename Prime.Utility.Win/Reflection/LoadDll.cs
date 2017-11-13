using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Utility.Win.Reflection
{
    /// <summary>
    /// https://stackoverflow.com/a/13355702/1318333
    /// </summary>
    public static class LoadDll
    {
        public class Proxy : MarshalByRefObject
        {
            public Assembly GetAssembly(string assemblyPath)
            {
                try
                {
                    return Assembly.LoadFile(assemblyPath);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public static Assembly Load(string filename)
        {
            if (!filename.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                throw new Exception("Cannot load assembly: " + filename + " as the file does not have the extension .dll");

            if (!File.Exists(filename))
                throw new Exception("Cannot load assembly: " + filename + " as the file cannot be found.");

            var domaininfo = new AppDomainSetup();
            var adevidence = AppDomain.CurrentDomain.Evidence;
            var domain = AppDomain.CreateDomain("MyDomain", adevidence, domaininfo);

            var type = typeof(Proxy);
            var value = (Proxy) domain.CreateInstanceAndUnwrap(type.Assembly.FullName, type.FullName);

            return value.GetAssembly(filename);
        }
    }
}

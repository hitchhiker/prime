using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading;
using Prime.Core;

namespace Prime.Extensions
{
    public class ExtensionLoader
    {
        [ImportMany]
        public IEnumerable<Lazy<IExtensionExecute>> ExtensionsExecute { get; set; }

        public void Compose()
        {
            var path = "V:\\prime\\src\\publish\\stage\\ipfs-3575ddcb0d8647d75fbf044c\\1.3.0-winamd64";
            var dir = new DirectoryInfo(path);

            var files = dir.GetFiles("*.dll", SearchOption.AllDirectories).AsEnumerable();
            //files = files.Where(x => !x.Name.StartsWith("Microsoft.", StringComparison.OrdinalIgnoreCase));
            //files = files.Where(x => !x.Name.StartsWith("System.", StringComparison.OrdinalIgnoreCase));

            Assembly LoadAsm(FileInfo x)
            {
                try { return AssemblyLoadContext.Default.LoadFromAssemblyPath(x.FullName); }
                catch { return null; }
            }

            var assemblies = files.Select(LoadAsm).Where(x=>x!=null).ToList();

            var types = new List<Type>();

            var extT = typeof(IExtension);

            foreach (var a in assemblies)
                types.AddRange(a.GetLoadableTypes().Where(x=> x.IsClass && !x.IsAbstract && !x.IsInterface && extT.IsAssignableFrom(x)).ToList());

            var configuration = new ContainerConfiguration();
            configuration.WithAssemblies(types.Select(x=>x.Assembly).Distinct());

            using (var container = configuration.CreateContainer())
            {
                ExtensionsExecute = container.GetExports<Lazy<IExtensionExecute>>();
            }

            var m = DefaultMessenger.I.Default;

            foreach (var i in ExtensionsExecute)
                i.Value.Main(new Core.PrimeContext(m));

            m.RegisterAsync<IpfsVersionResponse>(this, x => { Console.WriteLine(x.Version); });
            m.Send(new IpfsVersionRequest());

            Thread.Sleep(5000);
        }

        private void ComposeSafe()
        {
            /*
            var di = new DirectoryInfo(Server.MapPath("../../bin/"));

            if (!di.Exists) throw new Exception("Folder not exists: " + di.FullName);

            var dlls = di.GetFileSystemInfos("*.dll");
            AggregateCatalog agc = new AggregateCatalog();

            foreach (var fi in dlls)
            {
                try
                {
                    var ac = new AssemblyCatalog(Assembly.LoadFile(fi.FullName));
                    var parts = ac.Parts.ToArray(); // throws ReflectionTypeLoadException 
                    agc.Catalogs.Add(ac);
                }
                catch (ReflectionTypeLoadException ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                }
            }

            CompositionContainer cc = new CompositionContainer(agc);

            _providers = cc.GetExports<IDataExchangeProvider>();*/
        }
    }
}

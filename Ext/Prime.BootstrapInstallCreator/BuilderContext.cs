using System.IO;
using Prime.Base;
using Prime.Core;

namespace Prime.BootstrapInstallCreator
{
    public class BuilderContext
    {
        public BuilderContext(PrimeContext c)
        {
            C = c;
            BuildDir = C.FileSystem.GetTmpSubDirectory("build-bootstrapper", ObjectId.NewObjectId().ToString());
        }

        public readonly PrimeContext C;
        public readonly DirectoryInfo BuildDir;
        public string ProjectDir { get; set; }
        public string TemplateKey { get; set; }
        public string OsKey { get; set; }
    }
}
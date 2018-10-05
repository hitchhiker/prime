using System;
using System.IO;
using Prime.Base;
using Prime.Core;
using Prime.NetCoreExtensionPackager.Compiler;

namespace Prime.BootstrapInstallCreator
{
    public class Builder : CommonBase
    {
        private readonly BuilderContext _c;

        public Builder(BuilderContext builderContext) : base(builderContext.C)
        {
            _c = builderContext;
        }

        public bool Build()
        {
            if (!NetCoreNativeCompiler.Compile(C, _c.ProjectDir, _c.BuildDir.FullName, _c.OsKey))
            {
                L.Error($"Problem during compilation of {_c.ProjectDir}");
                return false;
            }

            if (!CopyResources())
            {
                L.Error($"Unable to copy resources from {_c.TemplateKey}.");
                return false;
            }
            return false;
        }

        private bool CopyResources()
        {
            //todo: stopped here
            return false;
        }
    }
}

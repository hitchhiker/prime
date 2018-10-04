using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Nito.AsyncEx;
using Prime.Core;

namespace Prime.NetCoreExtensionPackager.Compiler
{
    /// <summary>
    /// This is a temporary solution.
    /// </summary>
    public static class NetCoreNativeCompiler
    {
        public static bool Compile(PrimeContext context, string projectPath, string destinationPath)
        {
            if (Directory.Exists(destinationPath))
                Directory.Delete(destinationPath, true);

            var b = new List<string>();
            var result = AsyncContext.Run(() => Process("dotnet", $"publish {projectPath} -c Release -o {destinationPath}", b));
            var success= result == 0;
            if (success)
                return true;

            foreach (var line in b)
                context.L.Error(line);

            return false;
        }

        private static async Task<int> Process(string exe, string args, List<string> builder)
        {
            var psi = new ProcessStartInfo(exe, args);
            return await psi.StartAsync(builder.Add).ConfigureAwait(false);
        }
    }
}

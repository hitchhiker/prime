using System;
using System.IO;
using NetCrossRun.Core;
using Prime.Core;

namespace Prime
{
    public class PrimeWeb
    {
        public void StartWebConsole()
        {
            L.Log("Starting Prime Web Console server...");

            var primeWebFi = GetExecutable();
            L.Log("Executable found. Starting...");

            RunPrimeWebConsole(primeWebFi);
            
            L.Log("Prime Web Console started.");
        }

        private void RunPrimeWebConsole(FileInfo fi)
        {
            var webProcess = $"dotnet {fi.FullName}".ExecuteCommand(false, fi.DirectoryName);
        }

        private FileInfo GetExecutable()
        {
            const string executableName = "Prime.Web.dll";
            
            // TODO: change method of path getting.
            var relativePathPart = ExecuteOn.Os(
                () => "../../../../Prime.Web/bin/Release/netcoreapp2.0/publish", 
                () => "../../../../Prime.Web/bin/release/netcoreapp2.0/osx.10.12-x64/publish");
            
            var pathToFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePathPart);
            var fullName = Path.Combine(pathToFolder, executableName);
            
            var fi = new FileInfo(fullName);
            if(!fi.Exists)
                throw new FileNotFoundException("Prime.Web.dll not found.");
            
            return new FileInfo(fullName);
        }
        
        public ILogger L { get; set; }
    }
}
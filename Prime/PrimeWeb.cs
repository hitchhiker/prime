using System;
using System.Diagnostics;
using System.IO;
using System.Net.Mime;
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
        }

        private void RunPrimeWebConsole(FileInfo fi)
        {
            L.Log(fi);

            $"dotnet {fi.FullName}".ExecuteCommand(false, fi.DirectoryName).WaitForExit();
        }

        private FileInfo GetExecutable()
        {
            // TODO: change method of path getting.
            var pathToFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../Prime.Web/bin/release/netcoreapp2.0/osx.10.12-x64/publish");
            var executableName = "Prime.Web.dll";
            var fullName = Path.Combine(pathToFolder, executableName);
            
            var fi = new FileInfo(fullName);
            if(!fi.Exists)
                throw new FileNotFoundException("Prime.Web.dll not found.");
            
            return new FileInfo(fullName);
        }
        
        public ILogger L { get; set; }
    }
}
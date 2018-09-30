using System;
using System.Diagnostics;
using System.IO;
using NetCrossRun.Core;
using Prime.Core;

namespace Prime
{
    public class PrimeWeb
    {
        public void Run()
        {
            L.Log("Starting Prime Web server...");

            var primeWebFi = GetExecutable();
            L.Log("Executable found. Starting...");

            RunLocal(primeWebFi);
            
            L.Log("Prime Web started.");
        }

        private void RunLocal(FileInfo fi)
        {
            var webProcess = $"dotnet {fi.FullName}".ExecuteCommand(true, fi.DirectoryName);
            
            webProcess.OutputDataReceived += WebProcessOnOutputDataReceived;
            webProcess.ErrorDataReceived += WebProcessOnErrorDataReceived;
            
            webProcess.BeginOutputReadLine();
            webProcess.BeginErrorReadLine();
        }

        private void WebProcessOnErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            var data = e.Data;
            if (string.IsNullOrWhiteSpace(data))
                return;
            
            L.Error(data);
        }

        private void WebProcessOnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            var data = e.Data;
            
            if(data == null)
                return;
            
            if (data.IndexOf("Now listening on:", StringComparison.OrdinalIgnoreCase) != -1)
            {
                var url = GetUrlFromData(data);
                L.Log($"Prime Web is running at {url}.");
            }

            string GetUrlFromData(string outputData)
            {
                var startIndex = outputData.IndexOf("http", StringComparison.OrdinalIgnoreCase);
                var url = outputData.Substring(startIndex);

                return url;
            }
        }

        private FileInfo GetExecutable()
        {
            const string execName = "Prime.Web.dll";
            
            // TODO: change method of path getting.
            var relativePathPart = ExecuteOn.Os(
                () => "../../../../Prime.Web/bin/Release/netcoreapp2.0/publish", 
                () => "../../../../Prime.Web/bin/release/netcoreapp2.0/osx.10.12-x64/publish");
            
            var execFolderName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePathPart);
            var execFullPath = Path.Combine(execFolderName, execName);
            
            var fi = new FileInfo(execFullPath);
            if(!fi.Exists)
                throw new FileNotFoundException("Prime.Web.dll not found.");
            
            return new FileInfo(execFullPath);
        }
        
        public ILogger L { get; set; }
    }
}
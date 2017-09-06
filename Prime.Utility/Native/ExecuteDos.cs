using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Prime.Utility
{
    public class ExecuteDos
    {
        public class DosContext
        {
            public DosContext(string exe, string args = null)
            {
                Exe = exe;
                Args = args;
                TimeOut = new TimeSpan(0, 0, 0, 20);
            }

            public string Exe { get; private set; }

            public string Args { get; private set; }

            public bool NoWindow { get; set; }

            public bool Cancelled { get; set; }

            public TimeSpan TimeOut { get; set; }

            public Action<string> Logger { get; set; }

            public Action<string> ErrorLogger { get; set; }

            public bool AcceptBorkedExitCodes { get; set; }

            public Action<Process> OnProcessCreated { get; set; }

            public bool RedirectStandardInput { get; set; }

            public Action OnProcessEnded { get; set; }

            public Dictionary<string,string> Environment { get; private set; } = new Dictionary<string, string>();

            public override string ToString()
            {
                return "Exe: " + (Exe ?? "") + " Args: " + (Args ?? "");
            }
        }

        public Task<ProcessResult> CmdAsync(DosContext dosContext)
        {
            return new Task<ProcessResult>(()=> AsyncProcess(dosContext));
        }

        public class ProcessResult
        {
            public ProcessResult(DosContext dosContext, string str, int exitCode)
            {
                Output = str;
                Success = exitCode == 0 || (dosContext.AcceptBorkedExitCodes && (exitCode == 8 || exitCode == -1));
                ExitCode = exitCode;
            }

            public readonly string Output;
            public readonly bool Success;
            public readonly int ExitCode;
        }

        private ProcessResult AsyncProcess(DosContext c)
        {
            try
            {
                using (var process = new Process())
                {
                    foreach (var kv in c.Environment)
                    {
#if NETFX_45
                        process.StartInfo.EnvironmentVariables.Add(kv.Key, kv.Value);
#else
                        process.StartInfo.Environment.Add(kv);
#endif
                    }
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.FileName = c.Exe;
                    process.StartInfo.Arguments = c.Args;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.RedirectStandardInput = c.RedirectStandardInput;
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    process.StartInfo.WorkingDirectory = new FileInfo(c.Exe).DirectoryName;

                    var output = new StringBuilder();
                    var error = new StringBuilder();

                    process.OutputDataReceived += (_, e) => c.Logger?.Invoke(e.Data);
                    process.ErrorDataReceived += (_, e) => c.ErrorLogger?.Invoke(e.Data);
                    process.Exited += (_, __) => c.OnProcessEnded?.Invoke();

                    process.Start();

                    c.OnProcessCreated?.Invoke(process);

                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    var s = DateTime.UtcNow;
                    var insideTimeOut = true;

                    do
                    {
                        if (process.WaitForExit(10))
                            return new ProcessResult(c, output + error.ToString(), process.ExitCode);

                        insideTimeOut = (c.TimeOut == TimeSpan.MaxValue || s.AddMilliseconds(c.TimeOut.TotalMilliseconds) > DateTime.UtcNow);
                    } while (!c.Cancelled && insideTimeOut);

                    if (c.Cancelled)
                    {
                        try
                        {
                            if (!c.RedirectStandardInput)
                                process.Kill();
                            else
                                TryCtrlC(process);
                        }
                        catch { }
                    }

                    return new ProcessResult(c, output + error.ToString(), process.ExitCode);
                }
            }
            catch (Exception e)
            {
                throw new Exception("DOS error with: " + c + " " + e.Message, e);
            }
        }

        private void TryCtrlC(Process process)
        {
            try
            {
                ProcessExiter.Exit(process);
                if (!process.HasExited)
                    process.Kill();
            }
            catch (Exception e)
            {
                process.Kill();
            }
        }
    }
}

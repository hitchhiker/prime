using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using DnsClient;
using Prime.Spark;
using Prime.Utility;
using Timer = System.Timers.Timer;

namespace Prime.Radiant
{
    public class DeploymentManager
    {
        public readonly IpFsDaemon Daemon;
        public readonly IpFsApi Api;
        public readonly Dispatcher Dispatcher;
        public readonly Logger L;
        public DeploymentProvider Deployer;

        private readonly Action _raiseUi;
        private bool _raisedUi;

        public DeploymentManager(Dispatcher dispatcher, Logger logger, Action raiseUi)
        {
            Dispatcher = dispatcher;
            L = logger;
            _raiseUi = raiseUi;

            Daemon = new IpFsDaemon(this);
            Api = new IpFsApi(this);
        }

        public void Bootstrap()
        {
            var state = ProcessDeployment();

            if (state.RequiresRespark)
            {
                L.Info("Restarting Radiant");
                Shutdown(true);
                return;
            }

            if (state.IsCompleted)
            {
                L.Info("Boostrapping Prime");
                BootApplication();
                return;
            }
            
            L.Info("Something went wrong, try again.");
        }

        public event EventHandler OnDnsRetrieved;

        private IpfsDeployStatus ProcessDeployment()
        {
            var needsrespark = false;

            var state = GetProject("radiant", ()=> needsrespark = true);

            if (needsrespark)
                return IpfsDeployStatus.DoRespark;

            if (!state.IsCompleted)
                return state;

            return GetProject("prime");
        }

        private IpfsDeployStatus GetProject(string projectKey, Action onNewAssets = null)
        {
            var projectHash = RetrieveHashFromDns(projectKey);
            if (projectHash == null)
                return IpfsDeployStatus.DnsFailed;

            Deployer = new DeploymentProvider(projectKey, projectHash, L);

            if (Deployer.IsRetrieved)
            {
                L.Info(projectKey + " is up to date.");
                return IpfsDeployStatus.Completed;
            }

            if (!_raisedUi)
                _raiseUi.Invoke();

            _raisedUi = true;

            Daemon.Start();

            var state = TransferAssets();

            if (state.IsFailed)
                return state;

            onNewAssets?.Invoke();
            
            if (state.IsCompleted)
                L.Info(projectKey + " is updated.");

            return state;
        }

        private IpfsDeployStatus TransferAssets()
        {
            try
            {
                return Daemon.WaitTillRunning(client => Api.GetDeployment(Deployer) ? IpfsDeployStatus.Completed : IpfsDeployStatus.Failed);
            }
            catch (Exception e)
            {
                Fatal(e.Message);
            }
            return IpfsDeployStatus.Failed;
        }

        private string RetrieveHashFromDns(string projectKey)
        {
            L.Info("Syncronising " + projectKey + " via Radiant");

            L.Info("Resolving DNS [classical internet]");

            var dnsEntry = projectKey + "-" + Radiant.DnsVersion + ".getprime.org";

            OnDnsRetrieved?.Invoke(dnsEntry, EventArgs.Empty);

            var client = new LookupClient();
            var response = client.Query(dnsEntry, QueryType.TXT);
            var hash = response?.Answers?.TxtRecords()?.FirstOrDefault()?.Text?.FirstOrDefault();

            if (string.IsNullOrWhiteSpace(hash))
            {
                Fatal("Could not find " + projectKey +" HASH from DNS.");
                return null;
            }

            L.Info("Node: " + hash);

            OnDnsRetrieved?.Invoke(hash, EventArgs.Empty);

            return hash;
        }

        private void Fatal(string txt)
        {
            void WriteLog()
            {
                if (!string.IsNullOrEmpty(txt))
                    this.L.Error(txt + Environment.NewLine);

                this.L.Error("Unstable trying again..");

                var t = new Timer { Interval = 5000 };
                t.Elapsed += delegate { Dispatcher.Invoke(() => { Application.Current.Shutdown(); }); };
                t.Enabled = true;
            }

            Dispatcher.BeginInvoke((Action)WriteLog);
        }

        private string GetSparkExe()
        {
            var fileinfo = new FileInfo(Application.ResourceAssembly.Location);
            var dirinfo = fileinfo.Directory;
            var spark = new FileInfo(Path.Combine(dirinfo.FullName, Constants.SparkExeName));
            return spark.FullName;
        }

        public void BootApplication()
        {
            System.Diagnostics.Process.Start(Path.Combine(Deployer.BinDirectory.FullName, Constants.PrimeExeName));

            Dispatcher.Invoke(() =>
            {
                Application.Current.Shutdown();
            });
        }

        private void Shutdown(bool restart = false)
        {
            if (restart)
                System.Diagnostics.Process.Start(GetSparkExe());

            Dispatcher.Invoke(() =>
            {
                Application.Current.Shutdown();
            });
        }
    }
}

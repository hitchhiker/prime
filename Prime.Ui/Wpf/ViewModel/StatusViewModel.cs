using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prime.Radiant;

namespace Prime.Ui.Wpf.ViewModel
{
    public class StatusViewModel : INotifyPropertyChanged
    {
        private readonly DeploymentManager _manager;
        private readonly IpFsDaemon _ipfsDaemon;

        public StatusViewModel()
        {
            //_manager = manager;
            //_ipfsDaemon = _manager.Daemon;
            IpfsStatus = "Idle";
            DnsStatus = "Idle";
            _ipfsDaemon.OnStateChanged += _ipfsDaemon_OnStateChanged;
            _manager.OnDnsRetrieved += _manager_OnDnsRetrieved;
        }

        private void _manager_OnDnsRetrieved(object sender, EventArgs e)
        {
            if (sender is string hash)
                DnsStatus = hash;
        }

        private void _ipfsDaemon_OnStateChanged(object sender, EventArgs e)
        {
            IpfsStatus = _ipfsDaemon.State.ToString().ToLower();
        }

        private string _ipfsStatus;
        private string _dnsStatus;
        public event PropertyChangedEventHandler PropertyChanged;

        public string IpfsStatus
        {
            get => _ipfsStatus;
            set
            {
                _ipfsStatus = value;
                OnPropertyChanged();
            }
        }

        public string DnsStatus
        {
            get => _dnsStatus;
            set
            {
                _dnsStatus = value;
                OnPropertyChanged();
            }
        }

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

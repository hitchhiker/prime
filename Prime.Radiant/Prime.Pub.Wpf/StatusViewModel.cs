using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prime.Radiant.Components;

namespace Prime.Radiant
{
    public class StatusViewModel : INotifyPropertyChanged
    {
        private readonly PublishManager _manager;
        private readonly IpFsDaemon _ipfsDaemon;

        public StatusViewModel() { }

        public StatusViewModel(PublishManager manager)
        {
            _manager = manager;
            _ipfsDaemon = _manager.IpfsDaemon;
            IpfsStatus = "Idle";
            _ipfsDaemon.OnStateChanged += _ipfsDaemon_OnStateChanged;
        }

        private void _ipfsDaemon_OnStateChanged(object sender, EventArgs e)
        {
            IpfsStatus = _ipfsDaemon.State.ToString().ToLower();
        }

        private string _ipfsStatus;

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

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

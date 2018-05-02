using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Radiant
{
    public class TerminalViewModel : INotifyPropertyChanged
    {
        private readonly ObservableCollection<string> _items;

        public TerminalViewModel()
        {
            _items = new ObservableCollection<string>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public IEnumerable<string> Items => _items;

        public void AddItem(string item)
        {
            if (item.StartsWith("{"))
            {
                item = item.Substring(1);
                _items.Remove(_items.Last());
            }
            _items.Add(item);
        }

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

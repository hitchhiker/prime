using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Ui.Wpf.ViewModel
{

    /// <summary>
    /// https://stackoverflow.com/a/16745054/1318333
    /// </summary>
    public class LogEntry : DispatchedPropertyChangedBase
    {
        public DateTime DateTime { get; set; }

        public int Index { get; set; }

        public string Message { get; set; }
    }
}

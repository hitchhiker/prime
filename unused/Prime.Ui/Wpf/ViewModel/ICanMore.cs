using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Ui.Wpf.ViewModel
{
    public interface ICanMore
    {
        void AddRequest(int currentPageIndex, int pageSize);
    }
}

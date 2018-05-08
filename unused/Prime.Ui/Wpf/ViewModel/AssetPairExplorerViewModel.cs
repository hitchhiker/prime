using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Prime.Common;

namespace Prime.Ui.Wpf.ViewModel
{
    public class AssetPairExplorerViewModel
    {
        public AssetPairExplorerViewModel(DataExplorerItemModel dataExplorerItemModel)
        {
            _context = UserContext.Current;
            Dispatcher = Application.Current.Dispatcher;
            this.DataExplorerItemModel = dataExplorerItemModel;
        }

        public DataExplorerItemModel DataExplorerItemModel { get; private set; }

        public readonly Dispatcher Dispatcher;
        private readonly UserContext _context;
    }
}

using GalaSoft.MvvmLight.Messaging;
using Prime.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Prime.Ui.Wpf.ViewModel
{
    public class BuySellViewModel : DocumentPaneViewModel
    {
        private readonly ScreenViewModel _model;

        public BuySellViewModel(ScreenViewModel model)
        {
            _model = model;
        }

        public override CommandContent Create()
        {
            return new SimpleContentCommand("buy sell");
        }
    }
}

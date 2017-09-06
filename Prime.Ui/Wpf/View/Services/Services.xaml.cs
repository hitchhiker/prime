using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Prime.Core;
using FirstFloor.ModernUI.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Prime.Ui.Wpf.ViewModel;
using Prime.Utility;
using Prime.Ui.Wpf.ExtensionMethods;

namespace Prime.Ui.Wpf
{
    /// <summary>
    /// Interaction logic for Services.xaml
    /// </summary>
    public partial class Services
    {
        public Services()
        {
            InitializeComponent();
        }

        private async void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var li = (sender as TextBlock)?.DataContext as ServiceLineItem;
            if (li == null)
                return;

            var mw = this.TryFindParent<MetroWindow>();
            if (mw == null)
                return;

            var dialog = Application.Current.Resources["ViewDialog"] as BaseMetroDialog;
            if (dialog == null)
                return;

            dialog.Height = 500;

            await mw.ShowMetroDialogAsync(dialog, new MetroDialogSettings {ColorScheme = MetroDialogColorScheme.Inverted, AnimateShow=false});

            var content = new ServiceEdit() {DataContext = new ServiceEditViewModel(li.Provider as INetworkProviderPrivate)};
            var c = dialog.FindChild<Canvas>("Content");
            c.Children.Clear();
            c.Children.Add(content);

            void DetectOutClick(object o, MouseButtonEventArgs args)
            {
                var hit = VisualTreeHelper.HitTest(dialog, Mouse.GetPosition(dialog)) != null;
                if (hit)
                    return;

                mw.HideMetroDialogAsync(dialog).ContinueWith(x => { mw.PreviewMouseDown -= DetectOutClick; });
                args.Handled = true;
            }

            mw.PreviewMouseDown += DetectOutClick;
        }
        
    }
}

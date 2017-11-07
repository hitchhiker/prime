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
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Prime.Common;
using Prime.Ui.Wpf.View.Data_Explorer;
using Prime.Ui.Wpf.ViewModel;

namespace Prime.Ui.Wpf.View
{
    /// <summary>
    /// Interaction logic for DataExplorer.xaml
    /// </summary>
    public partial class DataExplorer : UserControl
    {
        public DataExplorer()
        {
            InitializeComponent();
        }

        private async void UIElement_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

            if (!((sender as StackPanel)?.DataContext is DataExplorerItemModel li))
                return;

            var mw = this.TryFindParent<MetroWindow>();
            if (mw == null)
                return;

            if (!(Application.Current.Resources["ViewDialog"] is BaseMetroDialog dialog))
                return;

            dialog.Height = 400;

            await mw.ShowMetroDialogAsync(dialog, new MetroDialogSettings { ColorScheme = MetroDialogColorScheme.Inverted, AnimateShow = false });

            var content = new AssetPairExplorer() { DataContext = new AssetPairExplorerViewModel(li) };
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

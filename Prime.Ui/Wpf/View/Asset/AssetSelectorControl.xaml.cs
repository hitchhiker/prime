using Prime.Ui.Wpf.ViewModel;
using System.Windows.Controls;

namespace Prime.Ui.Wpf.View.Asset
{
    /// <summary>
    /// Interaction logic for AssetSelectorControl.xaml
    /// </summary>
    public partial class AssetSelectorControl : ComboBox
    {
        public AssetSelectorControl()
        {
            InitializeComponent();
            this.DataContext = new AssetSelectorControlViewModel();
        }
    }
}

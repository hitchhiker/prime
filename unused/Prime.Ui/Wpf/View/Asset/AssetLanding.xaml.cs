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
using Prime.Ui.Wpf.ViewModel;

namespace Prime.Ui.Wpf.Pages
{
    /// <summary>
    /// Interaction logic for TradeBasic.xaml
    /// </summary>
    public partial class AssetLanding
    {
        public AssetLanding()
        {
            var ctx = UserContext.Current;
            var ba = ctx.BaseAsset;

            var ac = ctx.LastCommand as AssetGoCommand;
            var asset = ac?.Asset;

            if (asset == null || Equals(asset, Asset.None))
            {
                Visibility = Visibility.Collapsed;
                return;
            }

            Pair = new Core.AssetPair(asset, ba);

            this.DataContext = new LiveChartOhclViewModel();

            this.Initialized += TradeBasic_Initialized;
            InitializeComponent();

            if (UserContext.Current.UserSettings.Bookmarks.Contains(GetBookmark()))
                Star.Toggler.IsChecked = true;

            Star.Toggler.Checked += Toggler_Checked;
            Star.Toggler.Unchecked += Toggler_Unchecked;
        }

        private UserSetting Settings => UserContext.Current.UserSettings;

        private void TradeBasic_Initialized(object sender, EventArgs e)
        {
            ViewModel = (DataContext as LiveChartOhclViewModel);
            ViewModel.AssetPair = Pair;
            ViewModel.UpdateChartData();

            Head.Bind();
        }

        private void Toggler_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.Bookmarks.Remove(GetBookmark());
            Settings.Save(UserContext.Current);
        }

        private void Toggler_Checked(object sender, RoutedEventArgs e)
        {
            Settings.Bookmarks.Add(GetBookmark());
            Settings.Save(UserContext.Current);
        }

        private BookmarkBase GetBookmark()
        {
            return new CommandBookmark("asset " + Pair.Asset1, Pair.Asset1.AssetInfo.FullName + " (" + Pair.Asset1.ShortCode + ")");
        }

        public AssetPair Pair { get; set; }

        public LiveChartOhclViewModel ViewModel { get; set; }
    }
}

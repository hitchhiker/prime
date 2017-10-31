using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using Prime.Common;

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
            new Task(PopulateAssets).Start();
        }

        public void PopulateAssets()
        {
            int mostPopularCount = 0;

            List<ComboSectionItem> list = new List<ComboSectionItem>();

            var listAssets = Assets.I.Cached();

            mostPopularCount = listAssets.Count() > 5 ? 5 : listAssets.Count();

            for (int i = 0; i < mostPopularCount; i++)
            {
                list.Add(new ComboSectionItem { Header = "Most Popular", Asset = listAssets[i] });
            }

            if (mostPopularCount < listAssets.Count())
            {
                for (int i = mostPopularCount; i < listAssets.Count(); i++)
                {
                    list.Add(new ComboSectionItem { Header = "More Assets...", Asset = listAssets[i] });
                }
            }

            ListCollectionView listComboSections = new ListCollectionView(list);
            listComboSections.GroupDescriptions.Add(new PropertyGroupDescription("Header"));

            this.Dispatcher.Invoke(() =>
            {
                ItemsSource = listComboSections;
            });
        }
        
        public class ComboSectionItem
        {
            public string Header { get; set; }
            public Common.Asset Asset { get; set; }
        }
    }
}

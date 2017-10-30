using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Prime.Common;
using Prime.Ui.Wpf.View.Asset;
using Prime.Ui.Wpf.ViewModel.Trading;
using System.Windows;
using System.Windows.Threading;

namespace Prime.Ui.Wpf.ViewModel
{
    public class AssetSelectorControlViewModel : VmBase
    {
        public AssetSelectorControlViewModel()
        {
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

            ListComboSections = new ListCollectionView(list);
            ListComboSections.GroupDescriptions.Add(new PropertyGroupDescription("Header"));
        }

        public ListCollectionView ListComboSections { get; private set; }

        private ComboSectionItem _test;
        public ComboSectionItem Test
        {
            get => _test;
            set => Set(ref _test, value);
        }

        public class ComboSectionItem
        {
            public string Header { get; set; }
            public Asset Asset { get; set; }
        }
    }
}

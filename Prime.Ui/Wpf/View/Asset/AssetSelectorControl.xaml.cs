using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Prime.Common;
using System;

namespace Prime.Ui.Wpf.View.Asset
{
    /// <summary>
    /// Interaction logic for AssetSelectorControl.xaml
    /// </summary>
    public partial class AssetSelectorControl : ComboBox
    {
        private int _caretPosition;

        public AssetSelectorControl()
        {
            InitializeComponent();
            new Task(PopulateAssets).Start();
        }

        public void PopulateAssets()
        {
            var maxPopular = 8; //make it into a property.

            _listComboItems = new List<ComboSectionItem>();

            var listAssets = Assets.I.Cached();

            var popular = listAssets.Where(x => x.IsPopular).OrderBy(x => x.Popularity).Take(maxPopular).ToList();
            var regular = listAssets.Except(popular).Where(x=> !Equals(x, Common.Asset.None)).OrderBy(x => x.Name).ToList();

            foreach (var i in popular)
                _listComboItems.Add(new ComboSectionItem { Header = "Most Popular", Asset = i });
            
            foreach (var i in regular)
                _listComboItems.Add(new ComboSectionItem { Header = "More Assets...", Asset = i });
            
            /*
            if (mostPopularCount < listAssets.Count())
            {
                for (int i = mostPopularCount; i < listAssets.Count(); i++)
                {
                    _listComboItems.Add(new ComboSectionItem { Header = "More Assets...", Asset = listAssets[i] });
                }
            }*/

            ListCollectionView listComboSections = new ListCollectionView(_listComboItems);
            listComboSections.GroupDescriptions.Add(new PropertyGroupDescription("Header"));

            this.Dispatcher.Invoke(() =>
            {
                ItemsSource = listComboSections;
            });
        }

        private List<ComboSectionItem> _listComboItems;

        public class ComboSectionItem
        {
            public string Header { get; set; }
            public Common.Asset Asset { get; set; }
        }

        private void AssetSelectorControl_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.Text) == false)
            {
                var source = _listComboItems.Where(x =>
                    x.Asset.ShortCode.IndexOf(this.Text, StringComparison.InvariantCultureIgnoreCase) >= 0);
                ItemsSource = source;
                
                if (source.Any())
                {
                    IsDropDownOpen = true;
                }
                else
                {
                    IsDropDownOpen = false;
                }
            }
            else
            {
                PopulateAssets();
            }
        }

        private void AssetSelectorControl_OnDropDownOpened(object sender, EventArgs e)
        {
            PopulateAssets();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var element = GetTemplateChild("PART_EditableTextBox");
            if (element != null)
            {
                var textBox = (TextBox)element;
                textBox.SelectionChanged += OnDropSelectionChanged;
            }
        }

        private void OnDropSelectionChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            TextBox txt = (TextBox)sender;

            if (base.IsDropDownOpen && txt.SelectionLength > 0)
            {
                txt.CaretIndex = _caretPosition;
            }
            if (txt.SelectionLength == 0 && txt.CaretIndex != 0)
            {
                _caretPosition = txt.CaretIndex;
            }
        }
    }
}

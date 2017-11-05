using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Prime.Common;
using System;
using Prime.Ui.Wpf.ViewModel;
using Prime.Utility;

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
        }

        private void AssetSelectorControl_OnKeyUp(object sender, KeyEventArgs e)
        {
            CollectionView itemsViewOriginal = (CollectionView)CollectionViewSource.GetDefaultView(ItemsSource);

            itemsViewOriginal.Filter = ((comboItem) =>
            {
                if (string.IsNullOrWhiteSpace(Text)) return true;
                return ((Common.Asset)comboItem).ShortCode.IndexOf(Text, StringComparison.InvariantCultureIgnoreCase) >= 0;
            });
            
            IsDropDownOpen = itemsViewOriginal.Count > 0;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var element = GetTemplateChild("PART_EditableTextBox");
            if (element == null) return;
            var textBox = (TextBox)element;
            textBox.SelectionChanged += OnDropSelectionChanged;
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

using Prime.Core;
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

namespace Prime.Ui.Wpf.View.Exchange
{
    /// <summary>
    /// Interaction logic for ConversionResult.xaml
    /// </summary>
    public partial class ConversionResult : UserControl
    {
        public ConversionResult()
        {
            InitializeComponent();
            ResultArea.DataContext = this; //Very important
        }

        public static readonly DependencyProperty conversionDateTextProperty = DependencyProperty.Register("ConversionDate", typeof(string), typeof(ConversionResult), new FrameworkPropertyMetadata(""));
        public static readonly DependencyProperty assetLeftTextProperty = DependencyProperty.Register("AssetLeft", typeof(Asset), typeof(ConversionResult), new FrameworkPropertyMetadata(Asset.None));
        public static readonly DependencyProperty assetRightTextProperty = DependencyProperty.Register("AssetRight", typeof(Asset), typeof(ConversionResult), new FrameworkPropertyMetadata(Asset.None));
        public static readonly DependencyProperty convertLeftTextProperty = DependencyProperty.Register("ConvertLeft", typeof(double), typeof(ConversionResult), new FrameworkPropertyMetadata(double.MinValue));
        public static readonly DependencyProperty convertRightTextProperty = DependencyProperty.Register("ConvertRight", typeof(double), typeof(ConversionResult), new FrameworkPropertyMetadata(double.MinValue));

        public string ConversionDate
        {
            get { return (string)GetValue(conversionDateTextProperty); }
            set { SetValue(conversionDateTextProperty, value); }
        }

        public Asset AssetLeft
        {
            get { return (Asset)GetValue(assetLeftTextProperty); }
            set { SetValue(assetLeftTextProperty, value); }
        }

        public Asset AssetRight
        {
            get { return (Asset)GetValue(assetRightTextProperty); }
            set { SetValue(assetRightTextProperty, value); }
        }

        public double ConvertLeft
        {
            get { return (double)GetValue(convertLeftTextProperty); }
            set { SetValue(convertLeftTextProperty, value); }
        }

        public double ConvertRight
        {
            get { return (double)GetValue(convertRightTextProperty); }
            set { SetValue(convertRightTextProperty, value); }
        }
    }
}

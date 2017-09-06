using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using LiveCharts.Wpf.Charts.Base;

namespace prime
{
    public class PTextBox : TextBox
    {
        public static readonly DependencyProperty OnEnterCommandProperty = DependencyProperty.Register(nameof(OnEnterCommand), typeof(ICommand), typeof(PTextBox), new PropertyMetadata((object)null));

        public ICommand OnEnterCommand
        {
            get => (ICommand)this.GetValue(PTextBox.OnEnterCommandProperty);
            set => this.SetValue(PTextBox.OnEnterCommandProperty, (object)value);
        }

        public PTextBox()
        {
            PreviewKeyUp += OnKeyUp;
        }

        void OnKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != Key.Enter && e.Key != Key.Return)
                return;

            OnEnterCommand?.Execute(Text);
        }
    }
}
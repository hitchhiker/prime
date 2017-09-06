using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reactive;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Prime.Core;
using Prime.Ui.Wpf;
using Prime.Ui.Wpf.Annotations;
using Prime.Ui.Wpf.ExtensionMethods;
using Prime.Ui.Wpf.ViewModel;
using CommandManager = Prime.Core.CommandManager;

namespace prime
{

    /// <summary>
    /// Interaction logic for CustomCaretTextBox.xaml
    /// https://www.codeproject.com/Articles/633935/Customizing-the-Caret-of-a-WPF-TextBox
    /// </summary>
    public partial class AddressBox : UserControl
    {
        public string TestText { get; set; }

        public List<string> TestItems { get; set; }

        public AddressBoxModel Model => DataContext as AddressBoxModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressBox"/> class.
        /// </summary>
        public AddressBox()
        {
            InitializeComponent();
            if (!this.IsDesignMode())
                DataContextChanged += (o, args) => ContextChanged();
        }

        private void ContextChanged()
        {
            if (Model == null)
            {
                Visibility = Visibility.Hidden;
                return;
            }

            Model.PropertyChanged += Model_PropertyChanged;

            // Move custom caret whenever the selection has changed. (this includes typing, arrow keys, clicking)
            //
            CustomTextBox.SelectionChanged += (sender, e) => MoveCustomCaret();

            // Keep custom caret collpased until the text box has gained focus
            //
            CustomTextBox.LostFocus += (sender, e) => Caret.Visibility = Visibility.Collapsed;

            // Show custom caret as soon as text box has gained focus
            //
            CustomTextBox.GotFocus += (sender, e) => Caret.Visibility = Visibility.Visible;

            var window = Application.Current.MainWindow;

            window.Activated += (sender, args) => Canvas.Visibility = Visibility.Visible;
            window.Deactivated += (sender, args) => Canvas.Visibility = Visibility.Collapsed;


            MoveCustomCaret();
        }

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AddressBoxModel.LastCommandAccepted))
                OnCommandAccepted(Model.LastCommandAccepted);
        }

        private void OnCommandAccepted(CommandManagerEvent e)
        {
            CustomTextBox.Text = "";
            Watermark.Text = e?.Command?.Command.ToUpper();
            MoveCustomCaret();
            CustomTextBox.Focus();
        }

        /// <summary>
        /// Moves the custom caret on the canvas.
        /// </summary>
        private void MoveCustomCaret()
        {
            var caretLocation = CustomTextBox.GetRectFromCharacterIndex(CustomTextBox.CaretIndex).Location;

            if (!double.IsInfinity(caretLocation.X))
            {
                Canvas.SetLeft(Caret, caretLocation.X);
            }

            if (!double.IsInfinity(caretLocation.Y))
            {
                Canvas.SetTop(Caret, caretLocation.Y);
            }
        }
    }
}

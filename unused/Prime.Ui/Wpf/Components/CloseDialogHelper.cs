using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using MahApps.Metro.Controls.Dialogs;

namespace Prime.Ui.Wpf.ExtensionMethods
{
    /// <summary>
    /// https://www.devexpress.com/Support/Center/Question/Details/Q499558/click-outside-of-popup-suppress-mouse-click
    /// </summary>
    public static class CloseDialogHelper
    {
        public static bool OnPopupBorderControlPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var dialog = (BaseMetroDialog)sender;
            if (dialog == null)
                return false;

            if (VisualTreeHelper.HitTest(dialog, Mouse.GetPosition(dialog)) != null)
                return true;

            return false;
        }
    }
}
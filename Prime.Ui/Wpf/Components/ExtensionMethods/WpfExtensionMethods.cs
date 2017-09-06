using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using prime;

namespace Prime.Ui.Wpf.ExtensionMethods
{
    public static class WpfExtensionMethods
    {
        public static T FindAncestor<T>(this DependencyObject dependencyObject) where T : DependencyObject
        {
            var target = dependencyObject;
            do
            {
                target = VisualTreeHelper.GetParent(target);
            }
            while (target != null && !(target is T));
            return target as T;
        }

        public static bool IsDesignMode(this UIElement element)
        {
            return DesignerProperties.GetIsInDesignMode(element);
        }
    }
}

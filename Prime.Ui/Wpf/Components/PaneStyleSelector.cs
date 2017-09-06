using System.Windows;
using System.Windows.Controls;

namespace Prime.Ui.Wpf
{
    public class PaneStyleSelector : StyleSelector
    {
        public Style DocumentStyle { get; set; }
        public Style ToolStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is DocumentPaneViewModel)
                return DocumentStyle;

            if (item is ToolPaneViewModel)
                return ToolStyle;

            return base.SelectStyle(item, container);
        }
    }
}
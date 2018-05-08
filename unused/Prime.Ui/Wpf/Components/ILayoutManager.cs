
using System;
using Xceed.Wpf.AvalonDock;

namespace Prime.Ui.Wpf
{
    public interface ILayoutManager
    {
        void LoadLayout(DockingManager manager);
        void ResetLayout(DockingManager manager);
        void SaveLayout(DockingManager manager);
    }
}
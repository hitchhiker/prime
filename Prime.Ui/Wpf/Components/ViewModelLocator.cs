using Prime.Ui.Wpf.ViewModel;
using StructureMap;

namespace Prime.Ui.Wpf
{
    public class ViewModelLocator
    {
        private static readonly IContainer _container;

        static ViewModelLocator()
        {
            _container = new Container(new AppRegistry());
        }

        public static ScreenViewModel Screen => GetViewModel<ScreenViewModel>();

        public static LogPanelViewModel LogPanel => GetViewModel<LogPanelViewModel>();

        private static T GetToolViewModel<T>() where T : ToolPaneViewModel
        {
            return _container.GetInstance<T>();
        }

        private static T GetViewModel<T>()
        {
            // Get all viewmodels as unique instances
            return _container.GetInstance<T>();
        }
    }
}
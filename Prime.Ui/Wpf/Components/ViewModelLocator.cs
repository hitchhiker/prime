using Prime.Ui.Wpf.ViewModel;
using Prime.Utility;

namespace Prime.Ui.Wpf
{
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
        }

        public static ScreenViewModel Screen => GetViewModel<ScreenViewModel>();

        public static LogPanelViewModel LogPanel => GetViewModel<LogPanelViewModel>();


        private static T GetViewModel<T>() where T : class
        {
            return TypeCatalogue.I.GetInstance<T>();
        }
    }
}
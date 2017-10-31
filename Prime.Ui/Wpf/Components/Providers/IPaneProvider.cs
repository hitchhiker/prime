using Prime.Common;
using Prime.Ui.Wpf.ViewModel;

namespace Prime.Ui.Wpf
{
    public interface IPaneProvider
    {
        bool IsFor(CommandBase command);

        DocumentPaneViewModel GetInstance(ScreenViewModel model, CommandBase command);
    }
}
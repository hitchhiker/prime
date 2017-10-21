using Prime.Common;
using GalaSoft.MvvmLight.Messaging;
using Prime.Ui.Wpf.ViewModel;

namespace Prime.Ui.Wpf
{
    public interface IPaneProvider
    {
        bool IsFor(CommandBase command);

        DocumentPaneViewModel GetInstance(IMessenger messenger, ScreenViewModel model, CommandBase command);
    }
}
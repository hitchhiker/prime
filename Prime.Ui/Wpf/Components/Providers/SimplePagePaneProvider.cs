using Prime.Common;
using GalaSoft.MvvmLight.Messaging;
using Prime.Ui.Wpf.PageUri;
using Prime.Ui.Wpf.ViewModel;

namespace Prime.Ui.Wpf
{
    public class SimplePagePaneProvider : IPaneProvider
    {
        public bool IsFor(CommandBase command)
        {
            return command is SimpleContentCommand;
        }

        public DocumentPaneViewModel GetInstance(IMessenger messenger, ScreenViewModel model, CommandBase command)
        {
            return !(command is SimpleContentCommand c) ? null : GetInstance(messenger, model ,c);
        }

        private DocumentPaneViewModel GetInstance(IMessenger messenger, ScreenViewModel model, SimpleContentCommand command)
        {
            var panel = SimplePageUriProvider.GetViewModel(messenger, model, command);

            if (panel != null)
            {
                panel.CanClose = true;
                panel.IsActive = true;
                panel.IsSelected = true;
            }

            return panel;
        }
    }
}
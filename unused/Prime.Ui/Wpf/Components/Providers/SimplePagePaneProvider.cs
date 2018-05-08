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

        public DocumentPaneViewModel GetInstance(ScreenViewModel model, CommandBase command)
        {
            return !(command is SimpleContentCommand c) ? null : GetInstance(model ,c);
        }

        private DocumentPaneViewModel GetInstance(ScreenViewModel model, SimpleContentCommand command)
        {
            var panel = SimplePageUriProvider.GetViewModel(model, command);

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
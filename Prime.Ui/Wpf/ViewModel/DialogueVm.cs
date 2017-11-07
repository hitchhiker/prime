using GalaSoft.MvvmLight;
using MahApps.Metro.Controls.Dialogs;

namespace Prime.Ui.Wpf.ViewModel
{
    public class DialogueVm : VmBase
    {
        // Constructor
        public DialogueVm(IDialogCoordinator instance)
        {
            _dialogCoordinator = instance;
        }

        private readonly IDialogCoordinator _dialogCoordinator;

        // Methods
        private async void FooMessage()
        {
            await _dialogCoordinator.ShowMessageAsync(this, "HEADER", "MESSAGE");
        }

        private async void FooProgress()
        {
            // Show...
            var controller = await _dialogCoordinator.ShowProgressAsync(this, "HEADER", "MESSAGE");
            controller.SetIndeterminate();

            // Do your work... 

            // Close...
            await controller.CloseAsync();
        }

        // Actions... (ICommands for your view)
    }
}
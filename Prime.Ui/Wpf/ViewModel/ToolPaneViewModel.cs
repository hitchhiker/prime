using GalaSoft.MvvmLight;

namespace Prime.Ui.Wpf
{
    public abstract class ToolPaneViewModel : PaneViewModel
    {
        private bool _isVisible = true;

        private string _key;

        public string Key
        {
            get => _key;
            set
            {
                if (_key == value) return;
                _key = value;
                RaisePropertyChanged();
            }
        }

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible == value) return;
                _isVisible = value;
                RaisePropertyChanged();
            }
        }
    }
}
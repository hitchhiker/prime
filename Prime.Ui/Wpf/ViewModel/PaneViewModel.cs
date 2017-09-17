using System;
using GalaSoft.MvvmLight;
using Prime.Ui.Wpf.ViewModel;

namespace Prime.Ui.Wpf
{
    /// <summary>
    /// View model for a docking pane
    /// </summary>
    public abstract class PaneViewModel : VmBase, IDisposable
    {
        private bool _isSelected;
        private bool _isActive;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value) return;

                _isSelected = value;
                ((ObservableObject) this).RaisePropertyChanged();
            }
        }

        private string _title;
        public virtual string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }

        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive == value) return;
                _isActive = value;
                ((ObservableObject) this).RaisePropertyChanged();
            }
        }

        public virtual void Dispose()
        {
        }
    }
}
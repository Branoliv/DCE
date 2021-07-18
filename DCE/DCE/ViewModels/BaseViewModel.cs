using DCE.Services.Interface;
using DCE.Utils.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace DCE.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public IDialogAlertService DialogAlertService => DependencyService.Get<IDialogAlertService>();
        public IToastService ToastService => DependencyService.Get<IToastService>();
        public IDocumentService DocumentService => DependencyService.Get<IDocumentService>();
        public IPhotoService PhotoService => DependencyService.Get<IPhotoService>();
        public IConnectivityService ConnectivityService => DependencyService.Get<IConnectivityService>();
        public IMSGraphService MSGraphService => DependencyService.Get<IMSGraphService>();


        bool isEnable = true;
        public bool IsEnable
        {
            get => isEnable;
            set
            {
                SetProperty(ref isEnable, value);
                SetProperty(ref isEnable, value);
            }
        }

        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}

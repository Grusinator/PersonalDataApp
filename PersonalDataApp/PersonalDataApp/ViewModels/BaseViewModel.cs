using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Xamarin.Forms;

using PersonalDataApp.Models;
using PersonalDataApp.Services;
using PersonalDataApp.Views;

namespace PersonalDataApp.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {

        public GraphqlHandler GQLhandler = new GraphqlHandler();

        public IDataStore<Datapoint> DataStore => DependencyService.Get<IDataStore<Datapoint>>() ?? new DatapointDataStore(GQLhandler);

        User user = new User();
        public User User
        {
            get { return user; }
            set { SetProperty(ref user, value); }
        }

        bool isLoggedIn = false;
        public bool IsLoggedIn
        {
            get { return isLoggedIn; }
            set
            {
                SetProperty(ref isLoggedIn, value);
                if (value)
                {
                    GQLhandler.UpdateAuthToken(User.Token);
                }
                
            }
        }

        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }


        string title = string.Empty;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }


        public BaseViewModel()
        {
            
        }


        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName]string propertyName = "",
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

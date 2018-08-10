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
        public IDataStore<Datapoint> DataStore => DependencyService.Get<IDataStore<Datapoint>>() ?? new DatapointDataStore();

        public GraphqlHandler GQLhandler = new GraphqlHandler();

        User user = new User(); // { Username = "guest", Password = "test1234"};
        public User User
        {
            get { return user; }
            set
            {
                IsLoggedIn = user.Token == null ? false : true;
                SetProperty(ref user, value);
            }
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
        }        //bool isLoggedIn = false;
        //public bool isLoggedIn
        //{
        //    get { return isLoggedIn; }
        //    set { SetProperty(ref isLoggedIn, value); }
        //}


        //public PersonalDataApp.Models.User user
        //{
        //    get { return user; }
        //    set
        //    {
        //        if (user.token != null)
        //        {
        //            isLoggedIn = true;
        //            SetProperty(ref user, value);
        //        }
        //    }
        //}

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
            MessagingCenter.Subscribe<StartPage, User>(this, "BroadcastUser", (obj, user) => User = user);
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

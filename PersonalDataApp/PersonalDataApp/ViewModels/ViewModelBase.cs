using PersonalDataApp.Models;
using PersonalDataApp.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace PersonalDataApp.ViewModels
{
    public class ViewModelBase : BindableBase, INavigationAware, IDestructible
    {
        protected INavigationService NavigationService { get; private set; }
        protected IEventAggregator EventAggregator { get; private set; }


        public GraphqlHandler GQLhandler = new GraphqlHandler();

        public IDataStore<Datapoint> DataStore => DependencyService.Get<IDataStore<Datapoint>>() ?? new DatapointDataStore(GQLhandler);


        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

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

        string errorMessage = String.Empty;
        public string ErrorMessage
        {
            get { return errorMessage; }
            set { SetProperty(ref errorMessage, value); }
        }

        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }



        public ViewModelBase(INavigationService navigationService)
        {
            NavigationService = navigationService;
        }

        public virtual void OnNavigatedFrom(NavigationParameters parameters)
        {

        }

        public virtual void OnNavigatedTo(NavigationParameters parameters)
        {

        }

        public virtual void OnNavigatingTo(NavigationParameters parameters)
        {
            if (parameters.ContainsKey("user"))
            {
                User = (User)parameters["user"];
            }
            var a = Title;
        }

        public virtual void Destroy()
        {

        }
    }
}

using PersonalDataApp.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace PersonalDataApp.ViewModels
{
	public class SignupPageViewModel : ViewModelBase
	{
        public SignupPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
            : base(navigationService, eventAggregator)
        {

        }

        public DelegateCommand SignupCommand => new DelegateCommand(Signup);

        private async void Signup()
        {
            var _user = new User();

            IsBusy = true;
            //try to signup
            try
            {
                _user.Username = await GQLhandler.Signup(User.Username, User.Password, User.Email);
            }
            catch (HttpRequestException e)
            {
                ErrorMessage = "failed to create user: " + e.Message;
            }

            if(_user.Username == User.Username)
            {
                var p = new NavigationParameters() { { "user", User } };
                await NavigationService.NavigateAsync("LoginPage", p);
            }
        }
    }
}

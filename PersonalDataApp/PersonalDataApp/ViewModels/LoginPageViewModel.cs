using PersonalDataApp.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PersonalDataApp.ViewModels
{
	public class LoginPageViewModel : ViewModelBase
	{
        public LoginPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
            : base(navigationService, eventAggregator)
        {
            User.Username = "guest";
            User.Password = "test1234";
        }

        public DelegateCommand LoginCommand => new DelegateCommand(Login);

        private async void Login()
        {

            IsBusy = true;
            try
            {
                var Token = await GQLhandler.Login(User.Username, User.Password);
                User = await GQLhandler.GetUser();
                User.Token = Token;
            }
            catch (HttpRequestException e)
            {
                ErrorMessage = "failed: " + e.Message;
            }
            IsBusy = false;
            if (User.Token != null)
            {
                ErrorMessage = "success";

                var p = new NavigationParameters(){{"user", User}};
                await NavigationService.NavigateAsync("MainTabbedPage", p);
            }
        }
    }
}

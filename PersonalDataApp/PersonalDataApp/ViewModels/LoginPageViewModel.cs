using PersonalDataApp.Models;
using Prism.Commands;
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

        private ICommand _loginCommand;
        public ICommand LoginCommand => _loginCommand ?? (_loginCommand = new DelegateCommand(
            () => Task.Run(async () => await Login()))
        );


        public LoginPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {

        }

        private async Task Login()
        {
            var _user = new User();

            IsBusy = true;
            try
            {
                _user.Token = await GQLhandler.Login(User.Username, User.Password);
            }
            catch (HttpRequestException e)
            {
                ErrorMessage = "failed: " + e.Message;
            }
            IsBusy = false;
            if (_user.Token != null || true)
            {
                ErrorMessage = "success";
                User.Token = _user.Token;
                await NavigationService.NavigateAsync("PrismNavigationPage/MainTabbedPage");
                //EventAggregator.Send(this, "LoggedInUser", User);
            }
        }
    }
}

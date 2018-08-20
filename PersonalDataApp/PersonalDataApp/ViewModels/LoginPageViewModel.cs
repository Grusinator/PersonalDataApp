using PersonalDataApp.Models;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PersonalDataApp.ViewModels
{
	public class LoginPageViewModel : ViewModelBase
	{
        public LoginPageViewModel()
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
            if (_user.Token != null)
            {
                ErrorMessage = "success";
                User.Token = _user.Token;
                Application.Current.MainPage = new MainPage(User);
                //await Navigation.PopModalAsync();
                MessagingCenter.Send(this, "LoggedInUser", User);
            }
        }
    }
}

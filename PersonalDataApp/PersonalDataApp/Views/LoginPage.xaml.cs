using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using PersonalDataApp.Models;
using PersonalDataApp.Services;
using System.ComponentModel;
using System.Diagnostics;
using PersonalDataApp.ViewModels;
using System.Net.Http;

namespace PersonalDataApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
	{
        LoginViewModel viewModel;

        public LoginPage ()
		{
			InitializeComponent ();

            BindingContext = viewModel = new LoginViewModel();

            //viewModel.User = new User()
            //{
            //    Username = "guest",
            //    Password = "test1234",
            //    Token = "Dummy"
            //};
        }

        async void Login_Clicked(object sender, EventArgs e)
        {
            //MessagingCenter.Send(this, "UserLogin", user);
            await Login();
            //await Navigation.PopModalAsync();
        }


        private async Task Login()
        {
            var _user = new User();

            IsBusy = true;
            try
            {
                _user.Token = await viewModel.GQLhandler.Login(viewModel.User.Username, viewModel.User.Password);
            }
            catch(HttpRequestException e)
            {
                viewModel.ErrorMessage = "failed: " + e.Message;
            }
            IsBusy = false;
            if (_user.Token != null)
            {
                viewModel.ErrorMessage = "success";
                viewModel.User.Token = _user.Token;
                await Navigation.PopModalAsync();
                MessagingCenter.Send(this, "LoggedInUser", viewModel.User);
            }
        }
    }
}

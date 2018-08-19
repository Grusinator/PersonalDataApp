using PersonalDataApp.Models;
using PersonalDataApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PersonalDataApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SignupPage : ContentPage
	{

        LoginViewModel viewModel;

        public SignupPage ()
		{
			InitializeComponent ();

            BindingContext = viewModel = new LoginViewModel();
        }

        async void Signup_Clicked(object sender, EventArgs e)
        {
            await Signup();
        }

        private async Task Signup()
        {
            var _user = new User();

            viewModel.IsBusy = true;
            //try to signup
            try
            {
                _user.Username = await viewModel.GQLhandler.Signup(viewModel.User.Username, viewModel.User.Password, viewModel.User.Email);
            }
            catch(HttpRequestException e)
            {
                viewModel.ErrorMessage = "failed to create user: " + e.Message;
            }

            //if succeed, try to login also - should work ;)
            if (_user.Username == viewModel.User.Username)
            {
                viewModel.ErrorMessage = "user created, trying to login..";
                try
                {
                    _user.Token = await viewModel.GQLhandler.Login(viewModel.User.Username, viewModel.User.Password);
                }
                catch (HttpRequestException e)
                {
                    viewModel.ErrorMessage = "failed to login - try again later: " + e.Message;
                    await Task.Delay(2000);
                    await Navigation.PopModalAsync();
                }
                viewModel.IsBusy = false;
                //if succeed - 
                if (_user.Token != null)
                {
                    viewModel.User.Token = _user.Token;
                    viewModel.ErrorMessage = "success!";
                    await Task.Delay(500);
                    await Navigation.PopModalAsync();
                    MessagingCenter.Send(this, "LoggedInUser", viewModel.User);
                }
            }
        }
    }
}
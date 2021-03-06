﻿using System;
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

        }

        async void Login_Clicked(object sender, EventArgs e)
        {
            await Login();
        }


        private async Task Login()
        {
            var _user = new User();

            viewModel.IsBusy = true;
            try
            {
                _user.Token = await viewModel.GQLhandler.Login(viewModel.User.Username, viewModel.User.Password);
            }
            catch(HttpRequestException e)
            {
                viewModel.ErrorMessage = "failed: " + e.Message;
            }
            viewModel.IsBusy = false;
            if (_user.Token != null)
            {
                viewModel.ErrorMessage = "success";
                viewModel.User.Token = _user.Token;
                Application.Current.MainPage = new MainPage(viewModel.User);
                //await Navigation.PopModalAsync();
                MessagingCenter.Send(this, "LoggedInUser", viewModel.User);
            }
        }
    }
}

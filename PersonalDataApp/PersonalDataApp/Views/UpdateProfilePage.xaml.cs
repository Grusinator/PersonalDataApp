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
	public partial class UpdateProfilePage : ContentPage
	{
        ProfileViewModel viewModel;

        public UpdateProfilePage()
		{
			InitializeComponent ();
            BindingContext = viewModel = new ProfileViewModel();
        }

        public UpdateProfilePage(User user)
        {
            InitializeComponent();
            BindingContext = viewModel = new ProfileViewModel();
            viewModel.User = user;
            viewModel.GQLhandler.UpdateAuthToken(user.Token);
        }

        async void EditProfile_Clicked(object sender, EventArgs e)
        {
            await UpdateProfile();
        }


        private async Task UpdateProfile()
        {
            var _user = new User();

            viewModel.IsBusy = true;
            try
            {
                _user = await viewModel.GQLhandler.UpdateProfileAsync(viewModel.User);
            }
            catch (HttpRequestException e)
            {
                viewModel.ErrorMessage = "failed: " + e.Message;
                _user = null;
            }
            viewModel.IsBusy = false;
            if (_user != null)
            {
                viewModel.ErrorMessage = "success";
                await Navigation.PopModalAsync();
                MessagingCenter.Send(this, "UpdateProfile", _user);
            }
        }
    }
}
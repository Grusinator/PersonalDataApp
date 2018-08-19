using PersonalDataApp.Models;
using PersonalDataApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PersonalDataApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ProfilePage : ContentPage
	{

        ProfileViewModel viewModel;

        public ProfilePage()
		{
			InitializeComponent();

            BindingContext = viewModel = new ProfileViewModel();

            viewModel.User.Language = "Danish";

            MessagingCenter.Subscribe<UpdateProfilePage, User>(this, "UpdateProfile", (obj, user) =>
            {
                viewModel.User = user;
                viewModel.User.Name = "william";
            });
        }


        async void EditProfile_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new UpdateProfilePage(viewModel.User)));
        }
    }
}
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
        public User user { get; set; }

        ProfileViewModel viewModel;

        public ProfilePage()
		{
			InitializeComponent();

            BindingContext = viewModel = new ProfileViewModel();
		}



        async void EditProfile_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new UpdateProfilePage()));
        }

    }
}
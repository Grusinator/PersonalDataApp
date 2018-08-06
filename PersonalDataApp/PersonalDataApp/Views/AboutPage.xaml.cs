using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PersonalDataApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AboutPage : ContentPage
	{
		public AboutPage ()
		{
			InitializeComponent ();
		}

        async void LoginUser_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new LoginPage()));
        }

        async void SignupUser_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new SignupPage()));
        }
    }
}
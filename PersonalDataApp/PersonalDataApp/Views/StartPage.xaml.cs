using PersonalDataApp.Models;
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
	public partial class StartPage : ContentPage
	{
		public StartPage ()
		{
			InitializeComponent ();

            MessagingCenter.Subscribe<LoginPage, User>(this, "LoggedInUser", (obj, user) => ChangePageIfTokenIsNotNull(user));
            MessagingCenter.Subscribe<SignupPage, User>(this, "LoggedInUser", (obj, user) => ChangePageIfTokenIsNotNull(user));
        }

        private void ChangePageIfTokenIsNotNull(User user)
        {
            if (user.Token != null)
            {
                //Application.Current.MainPage = new MainPage(user);

                //send new User message here to new Main page
                MessagingCenter.Send(this, "BroadcastUser", user);
            }
        }

        async void Login_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new LoginPage()));
        }

        async void Signup_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new SignupPage()));
        }
    }
}
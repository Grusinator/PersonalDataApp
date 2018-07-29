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

namespace PersonalDataApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
	{
        public User user { get; set; }


        public LoginPage ()
		{
			InitializeComponent ();
            user = new User
            {
                Username = "guest",
                Password = "test1234"
            };

            BindingContext = this;
        }

        async void Login_Clicked(object sender, EventArgs e)
        {
            MessagingCenter.Send(this, "UserLogin", user);
            await Navigation.PopModalAsync();
        }
    }
}

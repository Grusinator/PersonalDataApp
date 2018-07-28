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

        public string Message
        {
            get { return Message; }
            set
            {
                ;
            }
        }

        public LoginPage ()
		{
			InitializeComponent ();
            user = new User
            {
                Username = "guest",
                Password = "test1234"
            };

            Message = "test";

            BindingContext = this;
        }

        async void Login_Clicked(object sender, EventArgs e)
        {
            GraphqlHandler handler = new GraphqlHandler();
            var token = await handler.Login(user.Username, user.Password);
            //IsBusy = true;

            if (token == null)
            {
                Message = "could not log in";
            }
            else
            {
                await Navigation.PopModalAsync();
            }
            //MessagingCenter.Send(this, "UserLogin", user);
            
        }
    }
}

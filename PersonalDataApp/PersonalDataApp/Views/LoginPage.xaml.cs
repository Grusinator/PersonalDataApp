using PersonalDataApp.Models;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PersonalDataApp.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        async void Login_Clicked(object sender, EventArgs e)
        {
            await Login();
        }


        
    }
}

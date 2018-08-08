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
	public partial class ProfilePage : ContentPage
	{
        public User user { get; set; }

        public ProfilePage()
		{
			InitializeComponent();
		}

        async void UpdateProfile_Clicked(object sender, EventArgs e)
        {
            MessagingCenter.Send(this, "UpdateProfile", user);
            await Navigation.PopModalAsync();
        }

    }
}
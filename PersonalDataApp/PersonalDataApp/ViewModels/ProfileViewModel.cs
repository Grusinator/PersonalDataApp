using PersonalDataApp.Models;
using PersonalDataApp.Views;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace PersonalDataApp.ViewModels
{
    class ProfileViewModel : BaseViewModel
    {

        public ProfileViewModel()
        {
            MessagingCenter.Subscribe<StartPage, User>(this, "BroadcastUser", (obj, user) =>
            {
                User = user;
                IsLoggedIn = true;
            });
        }
    }
}

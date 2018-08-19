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

        string errorMessage = String.Empty;
        public string ErrorMessage
        {
            get { return errorMessage; }
            set { SetProperty(ref errorMessage, value); }
        }

        public ProfileViewModel()
        {
            MessagingCenter.Subscribe<StartPage, User>(this, "BroadcastUser", (obj, user) =>
            {
                User = user;
                User.Name = "William1";
                User.Language = "Danish";
                IsLoggedIn = true;
            });

            MessagingCenter.Subscribe<UpdateProfilePage, User>(this, "UpdateProfile", (obj, user) =>
            {
                User = user;
                User.Name = "william2";
            });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using PersonalDataApp.Authentication;
using PersonalDataApp.Services;
using Xamarin.Forms;

namespace PersonalDataApp.Droid
{
    class IntentHandler : IIntentHandler
    {

        MainActivity MainActivity { get; set; }
        public GoogleAuthenticator Auth { get; set; }
        Intent Intent { get; set;  }

        public event EventHandler<EventArgs> AuthenticationSuccessfull;

        public IntentHandler(MainActivity mainActivity, GoogleAuthenticator auth)
        {
            MainActivity = mainActivity;
            Auth = auth;
        }

        public void StartIntent(GoogleAuthenticator Auth)
        {
            // Display the activity handling the authentication
            var authenticator = Auth.GetAuthenticator();
            Intent = authenticator.GetUI(MainActivity);
            MainActivity.StartActivity(Intent);
        }
    }
}
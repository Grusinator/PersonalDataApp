using PersonalDataApp.Authentication;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Auth;

namespace PersonalDataApp.Services
{
    public interface IIntentHandler
    {
        IThirdPartyDataProviderAuthenticator Auth { get; set; }
        void StartIntent(IThirdPartyDataProviderAuthenticator Auth);
    }


}

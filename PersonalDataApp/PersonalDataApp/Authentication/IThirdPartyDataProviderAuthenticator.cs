using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Auth;

namespace PersonalDataApp.Authentication
{
    public interface IThirdPartyDataProviderAuthenticator
    {
        void SetAuthDelegate(IAuthenticationDelegate authenticationDelegate);
        OAuth2Authenticator GetAuthenticator();
    }
}

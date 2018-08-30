using System;
using Xamarin.Auth;
using PersonalDataApp.Models;
using PersonalDataApp.Authentication;

namespace PersonalDataApp.Services.Authorization
{
    public class OAuthAuthenticator : IThirdPartyDataProviderAuthenticator
    {
        private const bool IsUsingNativeUI = false;

        private OAuth2Authenticator _auth;
        private IAuthenticationDelegate _authenticationDelegate;
        private ThirdPartyDataProvider _dataProvider;

        public OAuthAuthenticator(ThirdPartyDataProvider dataProvider)
        {
            _dataProvider = dataProvider;

            _auth = new OAuth2Authenticator(
                _dataProvider.ClientID, 
                _dataProvider.ClientSecret,
                _dataProvider.Scope,
                new Uri(_dataProvider.AuthorizeUrl),
                new Uri(_dataProvider.RedirectUrl),
                new Uri(_dataProvider.AccessTokenUrl),
                null, IsUsingNativeUI);

            _auth.Completed += OnAuthenticationCompleted;
            _auth.Error += OnAuthenticationFailed;
        }

        public void SetAuthDelegate(IAuthenticationDelegate authenticationDelegate)
        {
            _authenticationDelegate = authenticationDelegate;
        }

        public OAuth2Authenticator GetAuthenticator()
        {
            return _auth;
        }

        public void OnPageLoading(Uri uri)
        {
            _auth.OnPageLoading(uri);
        }

        private void OnAuthenticationCompleted(object sender, AuthenticatorCompletedEventArgs e)
        {
            if (e.IsAuthenticated)
            {
                var token = new OAuthToken
                {
                    TokenType = e.Account.Properties["token_type"],
                    AccessToken = e.Account.Properties["access_token"]
                };
                _authenticationDelegate.OnAuthenticationCompleted(token);
            }
            else
            {
                _authenticationDelegate.OnAuthenticationCanceled();
            }
        }

        private void OnAuthenticationFailed(object sender, AuthenticatorErrorEventArgs e)
        {
            _authenticationDelegate.OnAuthenticationFailed(e.Message, e.Exception);
        }
    }
}

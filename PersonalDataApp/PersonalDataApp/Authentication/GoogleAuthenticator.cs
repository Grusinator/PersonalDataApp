using System;
using Xamarin.Auth;

namespace PersonalDataApp.Authentication
{
    public class GoogleAuthenticator
    {
        //private const string AuthorizeUrl = "https://accounts.google.com/o/oauth2/v2/auth";
        //private const string AccessTokenUrl = "https://www.googleapis.com/oauth2/v4/token";
        //private const string ClientSecret = string.Empty;

        //nokia
        private const string AuthorizeUrl = "https://account.health.nokia.com/oauth2_user/authorize2";
        private const string AccessTokenUrl = "https://account.health.nokia.com/oauth2/token";

        private const bool IsUsingNativeUI = false;

        private OAuth2Authenticator _auth;
        private IGoogleAuthenticationDelegate _authenticationDelegate;

        public GoogleAuthenticator(string clientId, string scope, string redirectUrl)
        {
            _auth = new OAuth2Authenticator(clientId, Configuration.ClientSecret, scope,
                                            new Uri(Configuration.AuthorizeUrl),
                                            new Uri(Configuration.RedirectUrl),
                                            new Uri(Configuration.AccessTokenUrl),
                                            null, IsUsingNativeUI);

            _auth.Completed += OnAuthenticationCompleted;
            _auth.Error += OnAuthenticationFailed;
        }

        public void SetGoogleAuthDelegate(IGoogleAuthenticationDelegate authenticationDelegate)
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
                var token = new GoogleOAuthToken
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

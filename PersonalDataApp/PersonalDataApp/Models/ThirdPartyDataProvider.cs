using PersonalDataApp.Services.Authorization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace PersonalDataApp.Models
{
    public class ThirdPartyDataProvider
    {
        public string ProviderName { get; set; }
        public string AuthorizeUrl { get; set; }
        public string AccessTokenUrl { get; set; }
        public string ProfileRequestUrl { get; set; }
        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
        public string Scope { get; set; }
        public string RedirectUrl { get; set; }

        public bool IsConnected { get; set; } = false;
        public OAuthToken AccessToken { get; set; } 

        public static ObservableCollection<ThirdPartyDataProvider> InitDummyData()
        {
            return new ObservableCollection<ThirdPartyDataProvider>
            {
                new ThirdPartyDataProvider()
                {
                    ProviderName = "dummy",
                    AuthorizeUrl = "",
                    AccessTokenUrl = "",
                    ProfileRequestUrl = "",
                    ClientID = "",
                    ClientSecret = "",
                    Scope = "",
                    RedirectUrl = ""
                },
                new ThirdPartyDataProvider()
                {
                    ProviderName = "Endomondo",
                    AuthorizeUrl = "https://www.endomondo.com/oauth/authorize",
                    AccessTokenUrl = "https://api.endomondo.com/oauth/access_token",
                    ProfileRequestUrl = "https://api.endomondo.com/api/1/user",
                    ClientID = "",
                    ClientSecret = "",
                    Scope = "",
                    RedirectUrl = ""
                },
            new ThirdPartyDataProvider()
                {
                    ProviderName = "Nokia",
                    AuthorizeUrl = "https://account.health.nokia.com/oauth2_user/authorize2",
                    AccessTokenUrl = "https://account.health.nokia.com/oauth2/token",
                    ProfileRequestUrl = "",
                    ClientID = "a80378abe1059ef7c415cf79b09b1270f828c4a0fbfdc52dbec06ae5f71b4bb6",
                    ClientSecret = "1f1d852451385469a56ef6494cbd2e94c07421c3ee5ffbfca63216079fd36d1a",
                    Scope = "user.info",
                    RedirectUrl = "http://localhost:/oauth2redirect"
            },
                new ThirdPartyDataProvider()
                {
                    ProviderName = "Strava",
                    AuthorizeUrl = "https://www.strava.com/oauth/authorize",
                    AccessTokenUrl = "https://www.strava.com/oauth/token",
                    ProfileRequestUrl = "https://www.strava.com/api/v3/athlete",
                    ClientID = "28148",
                    ClientSecret = "ed5f469f798830c7214fc8efad54790799fc3ae1",
                    Scope = "view_private",
                    RedirectUrl = "http://localhost:/oauth2redirect"
                }
            };

        }
    }
}

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
                    Scope = "",
                    ClientID = "",
                    ClientSecret = "",
                    RedirectUrl = ""
                },
                new ThirdPartyDataProvider()
                {
                    ProviderName = "Strava",
                    AuthorizeUrl = "https://www.strava.com/oauth/authorize",
                    AccessTokenUrl = "https://www.strava.com/oauth/token",
                    ProfileRequestUrl = "https://www.strava.com/api/v3/athlete",
                    Scope = "view_private",
                    ClientID = "28148",
                    ClientSecret = "ed5f469f798830c7214fc8efad54790799fc3ae1",
                    RedirectUrl = "http://localhost:/oauth2redirect"
                }
            };

        }
    }
}

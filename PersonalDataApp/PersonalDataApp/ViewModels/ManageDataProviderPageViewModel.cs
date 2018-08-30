using PersonalDataApp.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using PersonalDataApp.Services.Authorization;
using PersonalDataApp.Authentication;
using System.Threading.Tasks;

namespace PersonalDataApp.ViewModels
{
	public class ManageDataProviderPageViewModel : ViewModelBase, IAuthenticationDelegate
    {

        public ThirdPartyDataProvider thirdPartyDataProvider = new ThirdPartyDataProvider();
        public ThirdPartyDataProvider ThirdPartyDataProvider
        {
            get { return thirdPartyDataProvider; }
            set { SetProperty(ref thirdPartyDataProvider, value); }
        }

        public OAuthAuthenticator OAuthAuthenticator;

        bool toogle = false;
        public bool Toogle
        {
            get { return toogle; }
            set
            {
                
                if (value)
                {
                    ConnectThirdPartyDataProvider();
                }
                else
                {
                    DisconnectThirdPartyDataProvider();
                }

                SetProperty(ref toogle, value);

            }
        }

        private void DisconnectThirdPartyDataProvider()
        {
            ;
        }

        private void ConnectThirdPartyDataProvider()
        { 
            OAuthAuthenticator = new OAuthAuthenticator(ThirdPartyDataProvider);

            OAuthAuthenticator.SetAuthDelegate(this);
            var IntentHandler = App.CreateIntentHandler();

            IntentHandler.StartIntent(OAuthAuthenticator);
        }

        public ManageDataProviderPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
            : base(navigationService, eventAggregator)
        {

        }

        public override void OnNavigatingTo(NavigationParameters parameters)
        {
            base.OnNavigatingTo(parameters);

            if (parameters.ContainsKey("thirdPartyDataProvider"))
            {
                ThirdPartyDataProvider = (ThirdPartyDataProvider)parameters["thirdPartyDataProvider"];
            }
        }

        public void OnAuthenticationCompleted(OAuthToken token)
        {
            ThirdPartyDataProvider.AccessToken = token;
            Task.Run(async () =>
            {
                GQLhandler.UpdateAuthToken(User.Token);
                string profileJsonField = await GetThirdPartyProfile.GetProfileAsync(ThirdPartyDataProvider);
                await GQLhandler.CreateThirdPartyProvider(ThirdPartyDataProvider, profileJsonField);
                IsBusy = false;
            });
            
        }

        public void OnAuthenticationFailed(string message, Exception exception)
        {
            ;
            
        }

        public void OnAuthenticationCanceled()
        {
            ;
        }
    }
}

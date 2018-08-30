using PersonalDataApp.Authentication;
using PersonalDataApp.Services;
using PersonalDataApp.Services.Authorization;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PersonalDataApp.ViewModels
{
	public class StartPageViewModel : ViewModelBase, IAuthenticationDelegate
	{
        public StartPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
            : base(navigationService, eventAggregator)
        {

        }

        IIntentHandler IntentHandler { get; set; }

        public DelegateCommand LoginCommand => new DelegateCommand(NavigateToLogin);
        public DelegateCommand SignupCommand => new DelegateCommand(NavigateToSignup);
        public DelegateCommand GoogleAuthCommand => new DelegateCommand(GoogleAuthenticate);



        public async void OnAuthenticationCompleted(OAuthToken token)
        {
            // Retrieve the user's email address
            // var googleService = new GoogleService();
            //var email = await googleService.GetEmailAsync(token.TokenType, token.AccessToken);


            NavigateToSignup();

        }

        public void OnAuthenticationCanceled()
        {

        }

        public void OnAuthenticationFailed(string message, Exception exception)
        {
        }

        private void GoogleAuthenticate()
        {
            IntentHandler = App.CreateIntentHandler();

            IntentHandler.Auth.SetAuthDelegate(this);

            IntentHandler.StartIntent(IntentHandler.Auth);
        }

        private async void NavigateToLogin()
        {
            await NavigationService.NavigateAsync("LoginPage");
        }

        private async void NavigateToSignup()
        {
            await NavigationService.NavigateAsync("SignupPage");
        }
    }
}

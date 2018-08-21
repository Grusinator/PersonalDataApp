using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PersonalDataApp.ViewModels
{
	public class StartPageViewModel : ViewModelBase
	{
        public StartPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
            : base(navigationService, eventAggregator)
        {

        }

        public DelegateCommand LoginCommand => new DelegateCommand(NavigateToLogin);
        public DelegateCommand SignupCommand => new DelegateCommand(NavigateToSignup);

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

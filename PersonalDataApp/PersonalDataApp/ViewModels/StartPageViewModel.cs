using Prism.Commands;
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
        public StartPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {

        }

        private ICommand _loginCommand;
        public ICommand LoginCommand => _loginCommand ?? (_loginCommand = new DelegateCommand(
            () => Task.Run(async () =>
            {
                await NavigationService.NavigateAsync("PrismNavigationPage/LoginPage");
            }))
        );

        private ICommand _signupCommand;
        public ICommand SignupCommand => _signupCommand ?? (_signupCommand = new DelegateCommand(
            () => Task.Run(async () =>
            {
                await NavigationService.NavigateAsync("PrismNavigationPage/SignupPage");
            }))
        );
    }
}

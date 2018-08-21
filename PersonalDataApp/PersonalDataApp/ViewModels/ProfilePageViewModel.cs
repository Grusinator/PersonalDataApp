using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PersonalDataApp.ViewModels
{
	public class ProfilePageViewModel : ViewModelBase
    {
        public ProfilePageViewModel(INavigationService navigationService)
            : base(navigationService)
        {

        }

        public DelegateCommand UpdateProfileCommand => new DelegateCommand(NavigateToUpdateProfile);

        private async void NavigateToUpdateProfile()
        {
            var p = new NavigationParameters() { { "user", User } };
            await NavigationService.NavigateAsync("UpdateProfilePage", p);
        }
    }
}

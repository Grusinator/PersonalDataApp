using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using PersonalDataApp.Models;

namespace PersonalDataApp.ViewModels
{
	public class ProfilePageViewModel : ViewModelBase
    {
        public ProfilePageViewModel(INavigationService navigationService)
            : base(navigationService)
        {

        }

        public override void OnNavigatingTo(NavigationParameters parameters)
        {
            base.OnNavigatingTo(parameters);

            EventAggregator.GetEvent<IsThresholdUpdated>().Publish(User.AudioThreshold);
        }
        

        public DelegateCommand UpdateProfileCommand => new DelegateCommand(NavigateToUpdateProfile);

        private async void NavigateToUpdateProfile()
        {
            var p = new NavigationParameters() { { "user", User } };
            await NavigationService.NavigateAsync("UpdateProfilePage", p);
        }
    }
}

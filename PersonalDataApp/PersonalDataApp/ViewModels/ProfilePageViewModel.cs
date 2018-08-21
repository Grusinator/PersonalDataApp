using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using PersonalDataApp.Models;
using Prism.Events;

namespace PersonalDataApp.ViewModels
{
	public class ProfilePageViewModel : ViewModelBase
    {
        public ProfilePageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
            : base(navigationService, eventAggregator)
        {
            
        }

        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

        }
        

        public DelegateCommand UpdateProfileCommand => new DelegateCommand(NavigateToUpdateProfile);

        private async void NavigateToUpdateProfile()
        {
            var p = new NavigationParameters() { { "user", User } };
            await NavigationService.NavigateAsync("UpdateProfilePage", p);
        }
    }
}

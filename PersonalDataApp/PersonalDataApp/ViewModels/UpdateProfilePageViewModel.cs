using PersonalDataApp.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PersonalDataApp.ViewModels
{
    public class UpdateProfilePageViewModel : ViewModelBase
    {
        public UpdateProfilePageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
            : base(navigationService, eventAggregator)
        {

        }

        public DelegateCommand UpdateProfileCommand => new DelegateCommand(UpdateProfile);

        private async void UpdateProfile()
        {
            User _user = new User();

            IsBusy = true;
            try
            {
                _user = await GQLhandler.UpdateProfileAsync(User);
            }
            catch (HttpRequestException e)
            {
                ErrorMessage = "failed: " + e.Message;
                _user = null;
            }
            IsBusy = false;
            if (_user != null)
            {
                //push an update to recorder with new threshold value
                EventAggregator.GetEvent<IsThresholdUpdated>().Publish(User.AudioThreshold);

                ErrorMessage = "success";
                var p = new NavigationParameters() { { "user", _user } };
                await NavigationService.GoBackAsync(p);
            }
        }
    }
}


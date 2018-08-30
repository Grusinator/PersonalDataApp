using PersonalDataApp.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;


namespace PersonalDataApp.ViewModels
{
	public class ThirdPartyDataProvidersPageViewModel : ViewModelBase
    {

        public ObservableCollection<ThirdPartyDataProvider> ThirdPartyDataProviders
            { get; set; } = ThirdPartyDataProvider.InitDummyData();

        public DelegateCommand LoadThirdPartyDataProvidersCommand => new DelegateCommand(LoadThirdPartyDataProviders);
        public DelegateCommand<ThirdPartyDataProvider> SelectDataProviderCommand
            => new DelegateCommand<ThirdPartyDataProvider>(OnItemSelected);

        public ThirdPartyDataProvidersPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
            : base(navigationService, eventAggregator)
        {

        }

        async void LoadThirdPartyDataProviders()
        {
            IsBusy = true;
            await Task.Delay(2000);
            ThirdPartyDataProviders = ThirdPartyDataProvider.InitDummyData();
            IsBusy = false;
        }

        async void OnItemSelected(ThirdPartyDataProvider thirdPartyDataProvider)
        {
            var p = new NavigationParameters() {
                { "thirdPartyDataProvider", thirdPartyDataProvider },
                { "user", User } 
        };
            await NavigationService.NavigateAsync("ManageDataProviderPage", p);
        }
    }
}

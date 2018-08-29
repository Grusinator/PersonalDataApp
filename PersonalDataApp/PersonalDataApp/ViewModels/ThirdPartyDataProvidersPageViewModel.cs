using PersonalDataApp.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PersonalDataApp.ViewModels
{
	public class ThirdPartyDataProvidersPageViewModel : ViewModelBase
    {
        public List<ThirdPartyDataProvider> ThirdPartyDataProviders = ThirdPartyDataProvider.InitDummyData();

        public DelegateCommand LoadThirdPartyDataProvidersCommand => new DelegateCommand(LoadThirdPartyDataProviders);
        public DelegateCommand<ThirdPartyDataProvider> ItemSelectedCommand 
            => new DelegateCommand<ThirdPartyDataProvider>(OnItemSelected);

        public ThirdPartyDataProvidersPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
            : base(navigationService, eventAggregator)
        {

        }

        async void LoadThirdPartyDataProviders()
        {
            ThirdPartyDataProviders = ThirdPartyDataProvider.InitDummyData();
        }

        async void OnItemSelected(ThirdPartyDataProvider thirdPartyDataProvider)
        {
            ;
        }
    }
}

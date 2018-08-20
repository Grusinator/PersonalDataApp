using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PersonalDataApp.ViewModels
{
	public class StartPageViewModel : ViewModelBase
	{
        public StartPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {

        }
	}
}

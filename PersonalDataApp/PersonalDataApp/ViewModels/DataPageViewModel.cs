using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PersonalDataApp.ViewModels
{
	public class DataPageViewModel : ViewModelBase
    {
        public DataPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {

        }
	}
}

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
	public class DatapointDetailPageViewModel : ViewModelBase
	{
        Datapoint datapoint = new Datapoint();
        public Datapoint Datapoint
        {
            get { return datapoint; }
            set { SetProperty(ref datapoint, value); }
        }

        public DatapointDetailPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
            : base(navigationService, eventAggregator)
        {

        }

        public override void OnNavigatingTo(NavigationParameters parameters)
        {
            base.OnNavigatingTo(parameters);

            if (parameters.ContainsKey("datapoint"))
            {
                Datapoint = (Datapoint)parameters["datapoint"];
            }
        }
    }
}

using PersonalDataApp.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PersonalDataApp.ViewModels
{
	public class DataPageViewModel : ViewModelBase
    {
        public ObservableCollection<Datapoint> Datapoints { get; set; } = new ObservableCollection<Datapoint>();

        public DelegateCommand LoadItemsCommand => new DelegateCommand(ExecuteLoadItemsCommand);
        public DelegateCommand<Datapoint> SelectDatapointCommand => new DelegateCommand<Datapoint>(OnItemSelected);


        public DataPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
            : base(navigationService, eventAggregator)
        {

        }

            

                //LoadItemsCommand.Execute(null);
         
            //MessagingCenter.Subscribe<AboutViewModel, Datapoint>(this, "AddDatapoint", async (obj, datapoint) =>
            //{
            //    var _datapoint = datapoint as Datapoint;
            //    Datapoints.Add(_datapoint);
            //    //OnPropertyChanged("Datapoints");
            //    await DataStore.AddItemAsync(_datapoint);
            //});


        async void ExecuteLoadItemsCommand()
        {
            if (IsBusy || !IsLoggedIn)
                return;

            IsBusy = true;

            try
            {
                Datapoints.Clear();
                var items = await DataStore.GetItemsAsync(true);
                foreach (var item in items)
                {
                    var _item = new Datapoint()
                    {
                        Category = item.Category,
                        TextFromAudio = item.TextFromAudio ?? "!null!",
                        Datetime = item.Datetime
                    };
                    Datapoints.Add(_item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }


        async void OnItemSelected(Datapoint datapoint)
        {
            var p = new NavigationParameters() { { "datapoint", datapoint } };
            await NavigationService.NavigateAsync("DatapointDetailPage", p);   
            
            
        }
    }
}

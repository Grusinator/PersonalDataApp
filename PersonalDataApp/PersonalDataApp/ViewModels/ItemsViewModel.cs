using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using PersonalDataApp.Models;
using PersonalDataApp.Views;
using System.Threading;

namespace PersonalDataApp.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        public ObservableCollection<Datapoint> Datapoints { get; set; }
        public Command LoadItemsCommand { get; set; }

        public ItemsViewModel()
        {
            Title = "Browse";
            Datapoints = new ObservableCollection<Datapoint>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());


            MessagingCenter.Subscribe<StartPage, User>(this, "BroadcastUser", (obj, user) =>
            {
                User = user;
                IsLoggedIn = true;

                LoadItemsCommand.Execute(null);
            });

            MessagingCenter.Subscribe<AboutViewModel, Datapoint>(this, "AddDatapoint", async (obj, datapoint) =>
            {
                var _datapoint = datapoint as Datapoint;
                Datapoints.Add(_datapoint);
                //OnPropertyChanged("Datapoints");
                await DataStore.AddItemAsync(_datapoint);
            });
        }

        private void OnUserLoaded(object sender, EventArgs e)
        {
            
        }

        async Task ExecuteLoadItemsCommand()
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
                    Datapoints.Add(item);
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
    }
}
using PersonalDataApp.Models;
using Prism.Navigation;
using Xamarin.Forms;

namespace PersonalDataApp.Views
{
    public partial class DataPage : ContentPage
    {
        public DataPage()
        {
            InitializeComponent();
        }


        //async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        //{
        //    var datapoint = args.SelectedItem as Datapoint;
        //    if (datapoint == null)
        //        return;

        //    var p = new NavigationParameters() { { "datapoint", datapoint } };
        //    await viewModel.NavigationService.NavigateAsync("DatapointDetailPage", p);

        //    // Manually deselect item.
        //    ItemsListView.SelectedItem = null;
        //}
        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using PersonalDataApp.Models;
using PersonalDataApp.Views;
using PersonalDataApp.ViewModels;

namespace PersonalDataApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ItemsPage : ContentPage
	{
        ItemsViewModel viewModel;

        public ItemsPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new ItemsViewModel();
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var datapoint = args.SelectedItem as Datapoint;
            if (datapoint == null)
                return;

            await Navigation.PushAsync(new ItemDetailPage(new ItemDetailViewModel(datapoint))); //change to item

            // Manually deselect item.
            ItemsListView.SelectedItem = null;
        }

        async void AddItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new NewItemPage()));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Datapoints.Count == 0 && false)
                viewModel.LoadItemsCommand.Execute(null);
        }
    }
}
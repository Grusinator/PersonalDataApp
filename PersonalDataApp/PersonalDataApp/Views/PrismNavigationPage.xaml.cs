using Prism.Navigation;
using Xamarin.Forms;

namespace PersonalDataApp.Views
{
    public partial class PrismNavigationPage : NavigationPage, INavigationAware
    {
        public PrismNavigationPage()
        {
            InitializeComponent();
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            if (parameters.ContainsKey("title"))
                Title = (string)parameters["title"];
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            
        }
    }
}

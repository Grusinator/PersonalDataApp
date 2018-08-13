using PersonalDataApp.Models;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PersonalDataApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainPage : TabbedPage
	{
        public User User { get; set; }

        public MainPage(User user)
        {
            InitializeComponent();

            User = user;
            //dont know how to access it

        }
	}
}
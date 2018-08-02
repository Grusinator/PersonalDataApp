using System;
using Xamarin.Forms;
using PersonalDataApp.Views;
using Xamarin.Forms.Xaml;
using PersonalDataApp.Services;


[assembly: XamlCompilation (XamlCompilationOptions.Compile)]
namespace PersonalDataApp
{
	public partial class App : Application
	{

        public static Func<IAudioRecorder> CreateAudioRecorder { get; set; }

        public App ()
		{
			InitializeComponent();

            MainPage = new MainPage();
        }
        protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}

using System;
using System.Windows.Input;

using Xamarin.Forms;

namespace PersonalDataApp.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel()
        {
            Title = "About";

            var recorder = App.CreateAudioRecorder();

            OpenWebCommand = new Command(() => Device.OpenUri(new Uri("https://xamarin.com/platform")));
            StartRecordingCommand = new Command(() => recorder.StartRecording());
        }

        public ICommand OpenWebCommand { get; }
        public ICommand StartRecordingCommand { get; }
    }
}
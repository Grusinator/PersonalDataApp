using System;
using System.Windows.Input;

using Xamarin.Forms;

namespace PersonalDataApp.ViewModels
{
    public class RecordAudioViewModel : BaseViewModel
    {
        public RecordAudioViewModel()
        {
            Title = "Record Audio";

            var recorder = App.CreateAudioRecorder();

            OpenWebCommand = new Command(() => Device.OpenUri(new Uri("https://xamarin.com/platform")));
            StartRecordingCommand = new Command(() => recorder.StartRecording());
        }

        public ICommand OpenWebCommand { get; }
        public ICommand StartRecordingCommand { get; }
    }
}
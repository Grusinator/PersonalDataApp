using Plugin.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Plugin.Permissions.Abstractions;


using Xamarin.Forms;

namespace PersonalDataApp.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel()
        {
            Title = "About";

            var recorder = App.CreateAudioRecorder();


            OpenWebCommand = new Command(() => RequestPermissions(
                new List<Permission>() {
                    Permission.Storage,
                    Permission.Microphone
                }
            ));

            StartRecordingCommand = new Command(() => recorder.StartRecording());
            StopRecordingCommand = new Command(() => recorder.StopRecording());

        }


        private async void RequestPermissions(List<Permission> permissions)
        {
            var requestedPermissions = await CrossPermissions.Current.RequestPermissionsAsync(permissions.ToArray());
            var requestedPermissionStatus = permissions.Select(p => requestedPermissions[p]);
        }

        public ICommand OpenWebCommand { get; }
        public ICommand StartRecordingCommand { get; }
        public ICommand StopRecordingCommand { get; }
    }
}
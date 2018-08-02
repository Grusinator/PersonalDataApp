using Plugin.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Plugin.Permissions.Abstractions;
using PersonalDataApp.Views;
using PersonalDataApp.Models;

using Xamarin.Forms;
using PersonalDataApp.Services;
using System.IO;

namespace PersonalDataApp.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        User user = new User();

        GraphqlHandler GQLhandler = new GraphqlHandler();

        private IAudioRecorder recorder { get; set; } 

        string userAction = string.Empty;
        public string UserAction
        {
            get { return userAction; }
            set { SetProperty(ref userAction, value); }
        }

        public User User
        {
            get { return user; }
            set
            {
                if (user.token != null)
                {
                    user = value;
                    SetProperty(ref user, value);
                }
            }
        }

        public AboutViewModel()
        {
            Title = "About";

            UserAction = "Login";

            recorder = App.CreateAudioRecorder();


            OpenWebCommand = new Command(() => RequestPermissions(
                new List<Permission>() {
                    Permission.Storage,
                    Permission.Microphone
                }
            ));

            StartRecordingCommand = new Command(() => StartRecording());
            StopRecordingCommand = new Command(() =>  StopRecording());

            MessagingCenter.Subscribe<LoginPage, User>(this, "UserLogin", async (obj, user) =>
            {
                var _user = user as User;

                IsBusy = true;
                _user.token = await GQLhandler.Login(user.Username, user.Password);
                if (_user.token != null)
                {
                    user = _user;
                    IsBusy = false;
                    UserAction = "Sign out";
                }
                else
                {
                    UserAction = "failed";
                }
                
            });

        }
        private void StartRecording()
        {
            recorder.StartRecording();

        }
        private void StopRecording()
        {
            string filepath = recorder.StopRecording();



            Datapoint obj = new Datapoint()
            {
                datetime = DateTime.Now,
                category = "test",
                source_device = "XamarinApp",
            };

            //var result = GQLhandler.uploadFile(filepath);
            //var result2 = GQLhandler.upload2Files(filepath, filepath);
            StartRecordingCommand = new Command(() => recorder.StartRecording());
            StopRecordingCommand = new Command(() => recorder.StopRecording());
            StartPlayingCommand = new Command(() => recorder.StartPlaying());
            StopPlayingCommand = new Command(() => recorder.StopPlaying());

            GQLhandler.UploadDatapoint(obj, filepath);
        }


        private async void RequestPermissions(List<Permission> permissions)
        {
            var requestedPermissions = await CrossPermissions.Current.RequestPermissionsAsync(permissions.ToArray());
            var requestedPermissionStatus = permissions.Select(p => requestedPermissions[p]);
        }

        public ICommand OpenWebCommand { get; }
        public ICommand StartRecordingCommand { get; }
        public ICommand StopRecordingCommand { get; }
        public ICommand StartPlayingCommand { get; }
        public ICommand StopPlayingCommand { get; }
    }
}
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
        

        GraphqlHandler GQLhandler = new GraphqlHandler();

        private List<Tuple<Datapoint, String>> GQLQueue = new List<Tuple<Datapoint, string>>();

        private IAudioRecorder recorder { get; set; }

        public ICommand OpenWebCommand { get; }
        public ICommand StartRecordingCommand { get; }
        public ICommand StopRecordingCommand { get; }
        public ICommand StartPlayingCommand { get; }
        public ICommand StopPlayingCommand { get; }

        string userAction = string.Empty;
        public string UserAction
        {
            get { return userAction; }
            set { SetProperty(ref userAction, value); }
        }

        User user = new User();
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

        string textFromAudio = string.Empty;
        public string TextFromAudio
        {
            get { return textFromAudio; }
            set { SetProperty(ref textFromAudio, value); }
        }

        bool enableStartRecord = false;
        public bool EnableStartRecord
        {
            get { return enableStartRecord; }
            set { SetProperty(ref enableStartRecord, value); }
        }

        bool enableStopRecord = false;
        public bool EnableStopRecord
        {
            get { return enableStopRecord; }
            set { SetProperty(ref enableStopRecord, value); }
        }

        bool enableStartPlay = false;
        public bool EnableStartPlay
        {
            get { return enableStartPlay; }
            set { SetProperty(ref enableStartPlay, value); }
        }

        bool enableStopPlay = false;
        public bool EnableStopPlay
        {
            get { return enableStopPlay; }
            set { SetProperty(ref enableStopPlay, value); }
        }


        public AboutViewModel()
        {
            Title = "About";

            UserAction = "Login";

            recorder = App.CreateAudioRecorder();

            RequestPermissions(
                new List<Permission>() {
                    Permission.Storage,
                    Permission.Microphone
                }
            );

            OpenWebCommand = new Command(() => RequestPermissions(
                new List<Permission>() {
                    Permission.Storage,
                    Permission.Microphone
                }
            ));

            StartRecordingCommand = new Command(() => StartRecording());
            StopRecordingCommand = new Command(() => StopRecording());
            StartPlayingCommand = new Command(() => StartPlayback());
            StopPlayingCommand = new Command(() => StopPlayback());

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
                    UpdateGuiReadyForRecording();
                }
                else
                {
                    UserAction = "failed";
                }
            });
        }

        private async void RequestPermissions(List<Permission> permissions)
        {
            var requestedPermissions = await CrossPermissions.Current.RequestPermissionsAsync(permissions.ToArray());
            var requestedPermissionStatus = permissions.Select(p => requestedPermissions[p]);
        }

        private void StartRecording()
        {
            UpdateGuiRecording();
            recorder.StartRecording();

        }
        private void StopRecording()
        {
            UpdateGuiReadyForRecordingOrPlayback();
            string filepath = recorder.StopRecording();

            Datapoint obj = new Datapoint()
            {
                datetime = DateTime.Now,
                category = "test",
                source_device = "XamarinApp",
            };

            //var result = GQLhandler.uploadFile(filepath);
            //var result2 = GQLhandler.upload2Files(filepath, filepath);

            IsBusy = true;
            try
            {
                obj = GQLhandler.UploadDatapoint(obj, filepath2:filepath);
                textFromAudio = obj.text_from_audio;
            }
            catch
            {
                GQLQueue.Add(new Tuple<Datapoint, string>(obj, filepath));
            }
            IsBusy = false;
            
        }
        private void StartPlayback()
        {
            UpdateGuiPlayback();
            recorder.StartPlaying();
        }
        private void StopPlayback()
        {
            UpdateGuiReadyForRecordingOrPlayback();
            recorder.StopPlaying();
        }

        private void UpdateGuiReadyForRecording()
        {
            EnableStartPlay = false;
            EnableStopPlay = false;
            EnableStartRecord = true;
            EnableStopRecord = false;
        }
        private void UpdateGuiRecording()
        {
            EnableStartPlay = false;
            EnableStopPlay = false;
            EnableStartRecord = false;
            EnableStopRecord = true;
        }
        private void UpdateGuiReadyForRecordingOrPlayback()
        {
            EnableStartPlay = true;
            EnableStopPlay = false;
            EnableStartRecord = true;
            EnableStopRecord = false;
        }
        private void UpdateGuiPlayback()
        {
            EnableStartPlay = false;
            EnableStopPlay = true;
            EnableStartRecord = false;
            EnableStopRecord = false;
        }
    }
}
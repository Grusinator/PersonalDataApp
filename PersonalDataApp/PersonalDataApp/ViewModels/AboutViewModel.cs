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
using System.Threading;
using System.Threading.Tasks;

namespace PersonalDataApp.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        private bool keepUploading;

        private List<Tuple<Datapoint, String>> GQLQueue = new List<Tuple<Datapoint, string>>();

        public IAudioRecorder recorder { get; set; }

        public ICommand OpenWebCommand { get; }
        public ICommand TestCommand { get; }
        public ICommand StartRecordingContinouslyCommand { get; }
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


        string someText = "None yet!";
        public string SomeText
        {
            get { return someText; }
            set { SetProperty(ref someText, value); }
        }


        User user = new User();
        public User User
        {
            get { return user; }
            set
            {
                if (user.Token != null)
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

        bool enableStartRecordContinously = false;
        public bool EnableStartRecordContinously
        {
            get { return enableStartRecordContinously; }
            set { SetProperty(ref enableStartRecordContinously, value); }
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


        bool isRecording = false;
        public bool IsRecording
        {
            get { return isRecording; }
            set { SetProperty(ref isRecording, value); }
        }

        string indicatorColor = "GREEN";
        public string IndicatorColor
        {
            get { return indicatorColor; }
            set { SetProperty(ref indicatorColor, value); }
        }

        double audioValue = 0.5;
        public double AudioValue
        {
            get { return audioValue; }
            set { SetProperty(ref audioValue, value); }
        }



        public AboutViewModel()
        {
            Title = "About";

            UserAction = "Login";

            textFromAudio = "";

            recorder = App.CreateAudioRecorder();

            recorder.RecordStatusChanged += UpdateRecordStatus;
            recorder.AudioReadyForUpload += UploadAudioData;

            

            //OpenWebCommand = new Command(() => RequestPermissions(
            //    new List<Permission>() {
            //        Permission.Storage,
            //        Permission.Microphone
            //    }
            //));

            //StartUploadScheduler();

            TestCommand = new Command(() => {
                RequestPermissions(
                    new List<Permission>() {
                        Permission.Storage,
                        Permission.Microphone
                    }
                );
            });
            
            StartRecordingContinouslyCommand = new Command(() => StartRecordingContinously());
            StartRecordingCommand = new Command(() => StartRecording());
            StopRecordingCommand = new Command(() => StopRecording());
            StartPlayingCommand = new Command(() => StartPlayback());
            StopPlayingCommand = new Command(() => StopPlayback());


            MessagingCenter.Subscribe<LoginPage, User>(this, "UserLogin", async (obj, user) =>
            {
                var _user = user as User;

                IsBusy = true;
                _user.Token = await GQLhandler.Login(user.Username, user.Password);
                if (_user.Token != null)
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

            MessagingCenter.Subscribe<SignupPage, User>(this, "UserSignup", async (obj, user) =>
            {
                var _user = user as User;

                IsBusy = true;
                _user.Username = await GQLhandler.Signup(user.Username, user.Password, user.Email);
                if (_user.Username != null)
                {
                    _user.Token = await GQLhandler.Login(user.Username, user.Password);
                }
                if (_user.Token != null)
                {
                    user = _user;
                    IsBusy = false;
                    UserAction = "Logout";
                    UpdateGuiReadyForRecording();
                }
                else
                {
                    UserAction = "Failed";
                }
            });
        }

        private void UploadAudioData(object sender, AudioRecorderGeneric.AudioUploadEventArgs e)
        {
            SomeText = UploadAudioDataPoint(e.Datetime, e.Filepath);
        }

        void UpdateRecordStatus(object sender, AudioRecorderGeneric.AudioDataEventArgs e)
        {
            IsRecording = e.AudioData.IsRecording ?? false;
            IndicatorColor = IsRecording ? "BLUE" : "RED";
            AudioValue = e.AudioData.Rms/1000;
        }

        private void StartUploadScheduler()
        {
            keepUploading = true;
            Task.Run(async () => {
                    while (keepUploading)
                    {
                        await Task.Delay(5000);
                        UploadAvailableData();
                    }
                }
            );
        }
        private void StopUploadScheduler()
        {
            keepUploading = false;
        }

        private void UploadAvailableData()
        {
            var donelist = new List<Tuple<DateTime, String>>();

            foreach (var elm in recorder.AudioFileQueue)
            {
                string audiotext = UploadAudioDataPoint(elm.Item1, elm.Item2);
                if (audiotext != null)
                {
                    donelist.Add(elm);
                }
            }
            donelist.ForEach(el => recorder.AudioFileQueue.Remove(el));
        }

        private string UploadAudioDataPoint(DateTime datetime, string filepath)
        {
            Datapoint obj = new Datapoint()
            {
                //Datetime = datetime,
                Category = "speech_audio",
                SourceDevice = "XamarinApp",
            };

            try
            {
                obj = GQLhandler.UploadDatapoint(obj, filepath2: filepath);
                return obj.TextFromAudio;
            }
            catch (Exception ex)
            {
                var str = ex.ToString();
            }

            return null;
        }

        private async void RequestPermissions(List<Permission> permissions)
        {
            var requestedPermissions = await CrossPermissions.Current.RequestPermissionsAsync(permissions.ToArray());
            var requestedPermissionStatus = permissions.Select(p => requestedPermissions[p]);
        }

        private void StartRecordingContinously()
        {
            UpdateGuiRecording();
            recorder.StartRecordingContinously();
        }

        private void StartRecording()
        {
            UpdateGuiRecording();
            recorder.StartRecording();
        }
        private void StopRecording()
        {
            UpdateGuiReadyForRecordingOrPlayback();
            recorder.StopRecording();
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
            EnableStartRecordContinously = true;
            EnableStartPlay = false;
            EnableStopPlay = false;
            EnableStartRecord = true;
            EnableStopRecord = false;
        }
        private void UpdateGuiRecording()
        {
            EnableStartRecordContinously = false;
            EnableStartPlay = false;
            EnableStopPlay = false;
            EnableStartRecord = false;
            EnableStopRecord = true;
        }
        private void UpdateGuiReadyForRecordingOrPlayback()
        {
            EnableStartRecordContinously = true;
            EnableStartPlay = true;
            EnableStopPlay = false;
            EnableStartRecord = true;
            EnableStopRecord = false;
        }
        private void UpdateGuiPlayback()
        {
            EnableStartRecordContinously = false;
            EnableStartPlay = false;
            EnableStopPlay = true;
            EnableStartRecord = false;
            EnableStopRecord = false;
        }
    }
}
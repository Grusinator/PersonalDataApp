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

        private static List<Permission> PermissionList = new List<Permission>() {
            Permission.Storage,
            Permission.Microphone
        };

    public IAudioRecorder recorder { get; set; }

        public ICommand OpenWebCommand { get; }
        public ICommand TestCommand { get; }
        public ICommand StartRecordingContinouslyCommand { get; }
        public ICommand StartRecordingCommand { get; }
        public ICommand StopRecordingCommand { get; }
        public ICommand StartPlayingCommand { get; }
        public ICommand StopPlayingCommand { get; }


        string someText = "None yet!";
        public string SomeText
        {
            get { return someText; }
            set { SetProperty(ref someText, value); }
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

        bool diableFileUpload = false;
        public bool DisableFileUpload
        {
            get { return diableFileUpload; }
            set { SetProperty(ref diableFileUpload, value); }
        }

        bool booleanSwitch = true;
        public bool BooleanSwitch
        {
            get { return booleanSwitch; }
            set { SetProperty(ref booleanSwitch, value); }
        }

        public AboutViewModel()
        {
            Title = "Recording";

            recorder = App.CreateAudioRecorder();

            recorder.RecordStatusChanged += UpdateRecordStatus;
            recorder.AudioReadyForUpload += UploadAudioData;

            //StartUploadScheduler();
            
            StartRecordingContinouslyCommand = new Command(() => StartRecordingContinously());
            StartRecordingCommand = new Command(() => StartRecording());
            StopRecordingCommand = new Command(() => StopRecording());
            StartPlayingCommand = new Command(() => StartPlayback());
            StopPlayingCommand = new Command(() => StopPlayback());

            UpdateGuiReadyForRecording();

            RequestPermissions(PermissionList);

            MessagingCenter.Subscribe<StartPage, User>(this, "BroadcastUser", (obj, user) =>
            {
                User = user;
                IsLoggedIn = true;
            });
        }

        private void UploadAudioData(object sender, AudioRecorderGeneric.AudioUploadEventArgs e)
        {
            Task.Run(async () => await UploadAudioDataAsync(e.Datetime, e.Filepath));
        }

        private async Task UploadAudioDataAsync(DateTime datetime, string filepath)
        {
            var datapoint = await UploadAudioDataPointAsync(datetime, filepath);
            if (datapoint.TextFromAudio == "")
            { datapoint.TextFromAudio = "is empty"; }
            SomeText = datapoint.TextFromAudio ?? "is null";
            //MessagingCenter.Send(this, "AddDatapoint", datapoint);
        }

        void UpdateRecordStatus(object sender, AudioRecorderGeneric.AudioDataEventArgs e)
        {
            IsRecording = e.AudioData.IsRecording ?? false;
            IndicatorColor = IsRecording ? "BLUE" : "RED";
            AudioValue = e.AudioData.Rms/1000;
            BooleanSwitch = e.AudioData.IsAllZeros;
        }

        private void StartUploadScheduler()
        {
            keepUploading = true;
            Task.Run(async () => {
                    while (keepUploading)
                    {
                        await Task.Delay(5000);
                        await UploadAvailableDataAsync();
                    }
                }
            );
        }
        private void StopUploadScheduler()
        {
            keepUploading = false;
        }

        private async Task UploadAvailableDataAsync()
        {
            var donelist = new List<Tuple<DateTime, String>>();

            foreach (var elm in recorder.AudioFileQueue)
            {
                var datapoint = await UploadAudioDataPointAsync(elm.Item1, elm.Item2);
                if (datapoint.TextFromAudio != null)
                {
                    donelist.Add(elm);
                }
            }
            donelist.ForEach(el => recorder.AudioFileQueue.Remove(el));
        }

        private async Task<Datapoint> UploadAudioDataPointAsync(DateTime datetime, string filepath)
        {
            Datapoint obj = new Datapoint()
            {
                //Datetime = datetime,
                Category = "speech_audio",
                SourceDevice = "XamarinApp",
            };

            try
            {
                GQLhandler.UpdateAuthToken(User.Token);

                if (DisableFileUpload)
                {
                    filepath = null;
                    return obj;
                }

                if (!File.Exists(filepath))
                {
                    ;
                }

                BooleanSwitch = true;
                var respObj = await GQLhandler.UploadDatapointAsync(obj, filepath2: filepath);

                BooleanSwitch = false;

                if (respObj == null)
                {
                    //try again, so much the file that is the problem, what is wrong?
                    respObj = await GQLhandler.UploadDatapointAsync(obj, filepath2: null);

                    if (File.Exists(filepath))
                    {
                        var bytes = File.ReadAllBytes(filepath);
                        var stringbyu = System.Text.Encoding.ASCII.GetString(bytes);
                        obj.TextFromAudio = "ERROR length: " + bytes.Length.ToString();
                        return obj;

                    }
                }
                return respObj;
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
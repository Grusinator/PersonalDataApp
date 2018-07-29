using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalDataApp.Services
{
    public interface IAudioRecorder
    {
        void StartPlaying();
        void StopPlaying();
        void StartRecording();
        string StopRecording();

        void SetStartPlayingButtons();
        void SetStopPlayingButtons();
        void SetStartRecordingButtons();
        void SetStopRecordingButtons();
    }
}

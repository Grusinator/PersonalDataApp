using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalDataApp
{
    public interface IAudioRecorder
    {
        void StartPlaying();
        void StopPlaying();
        void StartRecording();
        void StopRecording();

        void SetStartPlayingButtons();
        void SetStopPlayingButtons();
        void SetStartRecordingButtons();
        void SetStopRecordingButtons();
    }
}

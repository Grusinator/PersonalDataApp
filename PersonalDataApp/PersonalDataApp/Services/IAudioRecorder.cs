using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalDataApp.Services
{
    public interface IAudioRecorder
    {
        List<Tuple<DateTime, String>> AudioFileQueue { get; set; }

        event EventHandler<AudioRecorderGeneric.AudioDataEventArgs> RecordStatusChanged;

        
        void StartRecordingContinously();
        void StartPlaying();
        void StopPlaying();
        void StartRecording();
        void StopRecording();

    }
}

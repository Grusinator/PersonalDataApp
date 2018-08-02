using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;

namespace PersonalDataApp.Services
{
    public class AudioRecorderGeneric
    {
        //public static bool disableButtonEnabling = false;

        //public event PropertyChangedEventHandler PropertyChanged;

        public GraphqlHandler graphqlHandler;

        public static int sampleRate = 16000; //Herz
        public static int Nchannels = 1;
        public static long byteRate = 16 * sampleRate * Nchannels / 8;


        public AudioRecorderGeneric()
        {
            graphqlHandler = new GraphqlHandler();
        }



        //void OnPropertyChanged([CallerMemberName] string name = "")
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        //}

        //public bool EnableStartPlaying
        //{
        //    get { return EnableStartPlaying; }
        //    set
        //    {
        //        EnableStartPlaying = value;
        //        OnPropertyChanged();
        //    }
        //}
        //public bool EnableStopPlaying
        //{
        //    get { return EnableStopPlaying; }
        //    set
        //    {
        //        EnableStopPlaying = value;
        //        OnPropertyChanged();
        //    }
        //}
        //public bool EnableStartRecording
        //{
        //    get { return EnableStartRecording; }
        //    set
        //    {
        //        EnableStartRecording = value;
        //        OnPropertyChanged();
        //    }
        //}
        //public bool EnableStopRecording
        //{
        //    get { return EnableStopRecording; }
        //    set
        //    {
        //        EnableStopRecording = value;
        //        OnPropertyChanged();
        //    }
        //}

        //public void SetStartPlayingButtons()
        //{
        //    EnableStopPlaying = true;
        //    EnableStopRecording = false;
        //    EnableStartRecording = true;
        //}
        //public void SetStopPlayingButtons()
        //{
        //    EnableStartPlaying = true;
        //    EnableStopPlaying = false;
        //    EnableStopRecording = false;
        //    EnableStartRecording = true;
        //}
        //public void SetStartRecordingButtons()
        //{
        //    EnableStartPlaying = false;
        //    EnableStopRecording = true;
        //}
        //public void SetStopRecordingButtons()
        //{
        //    EnableStartPlaying = true;
        //    EnableStopPlaying = false;
        //    EnableStopRecording = false;
        //    EnableStartRecording = true;
        //}

        //public void initButtons()
        //{
        //    EnableStartPlaying = true;
        //    EnableStopPlaying = true;
        //    EnableStopRecording = true;
        //    EnableStartRecording = true;
        //}

        public void WriteWaveFileHeader(BinaryWriter bWriter, int totalAudioLen, int totalDataLen)
        {
            byte[] header = new byte[44];

            header[0] = (byte)'R'; // RIFF/WAVE header
            header[1] = (byte)'I';
            header[2] = (byte)'F';
            header[3] = (byte)'F';
            header[4] = (byte)(totalDataLen & 0xff);
            header[5] = (byte)((totalDataLen >> 8) & 0xff);
            header[6] = (byte)((totalDataLen >> 16) & 0xff);
            header[7] = (byte)((totalDataLen >> 24) & 0xff);
            header[8] = (byte)'W';
            header[9] = (byte)'A';
            header[10] = (byte)'V';
            header[11] = (byte)'E';
            header[12] = (byte)'f'; // 'fmt ' chunk
            header[13] = (byte)'m';
            header[14] = (byte)'t';
            header[15] = (byte)' ';
            header[16] = 16; // 4 bytes: size of 'fmt ' chunk
            header[17] = 0;
            header[18] = 0;
            header[19] = 0;
            header[20] = 1; // format = 1
            header[21] = 0;
            header[22] = (byte)Nchannels;
            header[23] = 0;
            header[24] = (byte)(sampleRate & 0xff);
            header[25] = (byte)((sampleRate >> 8) & 0xff);
            header[26] = (byte)((sampleRate >> 16) & 0xff);
            header[27] = (byte)((sampleRate >> 24) & 0xff);
            header[28] = (byte)(byteRate & 0xff);
            header[29] = (byte)((byteRate >> 8) & 0xff);
            header[30] = (byte)((byteRate >> 16) & 0xff);
            header[31] = (byte)((byteRate >> 24) & 0xff);
            header[32] = (byte)(2 * 16 / 8); // block align
            header[33] = 0;
            header[34] = 16; // bits per sample
            header[35] = 0;
            header[36] = (byte)'d';
            header[37] = (byte)'a';
            header[38] = (byte)'t';
            header[39] = (byte)'a';
            header[40] = (byte)(totalAudioLen & 0xff);
            header[41] = (byte)((totalAudioLen >> 8) & 0xff);
            header[42] = (byte)((totalAudioLen >> 16) & 0xff);
            header[43] = (byte)((totalAudioLen >> 24) & 0xff);

            bWriter.Write(header, 0, 44);
        }
    }
}

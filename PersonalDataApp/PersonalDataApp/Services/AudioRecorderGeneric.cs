using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;
using System.Linq;
using System.Numerics;
using PersonalDataApp.Models;

namespace PersonalDataApp.Services
{
    public class AudioRecorderGeneric 
    {
        //public static bool disableButtonEnabling = false;

        //public event PropertyChangedEventHandler PropertyChanged;

        public GraphqlHandler graphqlHandler;

        public static int sampleFrequency = 16000; //Herz
        public static int Nchannels = 1;
        public static long byteRate = 16 * sampleFrequency * Nchannels / 8;


        public event EventHandler<AudioDataEventArgs> RecordStatusChanged;
        public event EventHandler<AudioUploadEventArgs> AudioReadyForUpload;

        public AudioRecorderGeneric()
        {
            graphqlHandler = new GraphqlHandler();
        }

        public class AudioDataEventArgs : EventArgs
        {
            public AudioDataAnalysis AudioDataAnalysis { get; set; }

            public AudioDataEventArgs(AudioDataAnalysis audioDataAnalysis)
            {
                AudioDataAnalysis = audioDataAnalysis;
            }
        }
        public class AudioUploadEventArgs : EventArgs
        {
            public string Filepath{ get; set; }
            public DateTime Datetime { get; set; }

            public AudioUploadEventArgs( DateTime datetime, string filepath)
            {
                Filepath = filepath;
                Datetime = datetime;
            }
        }
        protected virtual void OnRecordStatusChanged(AudioDataEventArgs e)
        {
            RecordStatusChanged?.Invoke(this, e);
        }


        protected virtual void OnAudioReadyForUpload(AudioUploadEventArgs e)
        {
            AudioReadyForUpload?.Invoke(this, e);
        }

        private void WriteWavHeader(MemoryStream stream, bool isFloatingPoint, ushort channelCount, ushort bitDepth, int sampleFrequency, int totalSampleCount)
        {
            stream.Position = 0;

            // RIFF header.
            // Chunk ID.
            stream.Write(Encoding.ASCII.GetBytes("RIFF"), 0, 4);

            // Chunk size.
            stream.Write(BitConverter.GetBytes(((bitDepth / 8) * totalSampleCount) + 36), 0, 4);

            // Format.
            stream.Write(Encoding.ASCII.GetBytes("WAVE"), 0, 4);



            // Sub-chunk 1.
            // Sub-chunk 1 ID.
            stream.Write(Encoding.ASCII.GetBytes("fmt "), 0, 4);

            // Sub-chunk 1 size.
            stream.Write(BitConverter.GetBytes(16), 0, 4);

            // Audio format (floating point (3) or PCM (1)). Any other format indicates compression.
            stream.Write(BitConverter.GetBytes((ushort)(isFloatingPoint ? 3 : 1)), 0, 2);

            // Channels.
            stream.Write(BitConverter.GetBytes(channelCount), 0, 2);

            // Sample rate.
            stream.Write(BitConverter.GetBytes(sampleFrequency), 0, 4);

            // Bytes rate.
            stream.Write(BitConverter.GetBytes(sampleFrequency * channelCount * (bitDepth / 8)), 0, 4);

            // Block align.
            stream.Write(BitConverter.GetBytes((ushort)channelCount * (bitDepth / 8)), 0, 2);

            // Bits per sample.
            stream.Write(BitConverter.GetBytes(bitDepth), 0, 2);



            // Sub-chunk 2.
            // Sub-chunk 2 ID.
            stream.Write(Encoding.ASCII.GetBytes("data"), 0, 4);

            // Sub-chunk 2 size.
            stream.Write(BitConverter.GetBytes((bitDepth / 8) * totalSampleCount), 0, 4);
        }

        public void WriteWaveFileHeader(BinaryWriter bWriter, int totalAudioLen)
        {
            //correction for the header, im not sure why 36 - would have expected 44
            var totalDataLen = totalAudioLen + 36;

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
            header[24] = (byte)(sampleFrequency & 0xff);
            header[25] = (byte)((sampleFrequency >> 8) & 0xff);
            header[26] = (byte)((sampleFrequency >> 16) & 0xff);
            header[27] = (byte)((sampleFrequency >> 24) & 0xff);
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

        public static Int16[] ByteArrayTo16Bit(byte[] byteArray)
        {
            int intlength = byteArray.Length / 2;

            Int16[] output = new Int16[intlength];

            for (int i = 0; i < byteArray.Length; i = i + 2)
            {
                var index = (int)i / 2;
                output[index] = BitConverter.ToInt16(byteArray, i);
            }
            return output;
        }



    }
}

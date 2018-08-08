using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Media.Audiofx;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using System.Numerics;
using PersonalDataApp.Services;
using PersonalDataApp.Models;


namespace PersonalDataApp.Droid
{
    public class AudioRecorder : AudioRecorderGeneric, IAudioRecorder
    {
        static Android.Media.Encoding encoding = Android.Media.Encoding.Pcm16bit;
        static ChannelIn channelIn = ChannelIn.Mono;
        static ChannelOut channelOut = ChannelOut.Mono;

        //public event EventHandler<AudioDataEventArgs> RecordStatusChanged;

        static int minbufferSize = AudioRecord.GetMinBufferSize(
            sampleRateInHz: sampleRate, 
            channelConfig: channelIn, 
            audioFormat: encoding
        );

        static int maxAudioFreamesLength = 64000;

        static string externalDir = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath.ToString();

        static string personalpath = Android.OS.Environment.DirectoryMusic;
        static string audioDir = Path.Combine(externalDir, "PersonalData");


        public List<Tuple<DateTime, String>> AudioFileQueue { get; set; }

        public string status;

        public string wavPath;
        public bool isRecording;
        public bool containsVoice;
        public bool _forceStop;


        string pathSave { get; set; }

        AudioRecord audioRecord;
        AudioTrack audioTrack;


        public AudioRecorder(): base()
        {
            AudioFileQueue = new List<Tuple<DateTime, String>>();
            if (!Directory.Exists(audioDir))
            {
                Directory.CreateDirectory(audioDir);
            }
        }

        public List<Tuple<DateTime,String>> GetQueue()
        {
            return AudioFileQueue;
        }

        public void StartPlaying()
        {
            byte[] fileData = File.ReadAllBytes(wavPath);
            new Thread(delegate ()
            {
                PlayAudioTrack(fileData);
            }).Start();

        }

        public void StopPlaying()
        {
            if (audioTrack != null)
            {
                audioTrack.Stop();
                audioTrack.Release();
            }
        }

        public void StartRecording()
        {
            Task.Run(async () => await RecordAudioAsync());
        }

        public void StartRecordingContinously()
        {
            Task.Run(async () => await RecordAudiocontinuousAsync());
        }

        public void StopRecording()
        {
            //if recording continously 
            _forceStop = true;
            if (isRecording == true)
            {
                isRecording = false;
            }
        }


        private async Task RecordAudiocontinuousAsync()
        {

            byte[] audioBuffer = new byte[8000];

            byte[] preAudioBuffer = new byte[8000];

            audioRecord = new AudioRecord(
                AudioSource.Mic,// Hardware source of recording.
                sampleRate,// Frequency
                channelIn,// Mono or stereo
                encoding,// Audio encoding
                audioBuffer.Length// Length of the audio clip.
            );

            int totalAudioLen = 0;

            _forceStop = false;

            audioRecord.StartRecording();

            using (MemoryStream memory = new MemoryStream())
            using (BufferedStream stream = new BufferedStream(memory))
            {
                while (!_forceStop)
                {
                    //start listening
                    totalAudioLen += await audioRecord.ReadAsync(audioBuffer, 0, audioBuffer.Length);

                    //analysis
                    var intbuffer = ByteArrayTo16Bit(audioBuffer);

                    var audioData = new AudioData(intbuffer, isRecording);


                    containsVoice = audioData.IdentifyVoice();

                    OnRecordStatusChanged(new AudioDataEventArgs(audioData));


                    //if voice has been detected, start writing 
                    if (containsVoice && !isRecording)
                    {
                        totalAudioLen = 0;
                        isRecording = true;
                        stream.Write(audioBuffer, 0, audioBuffer.Length);

                    }
                    //if sound is still detected keep on recording
                    else if (containsVoice && isRecording)
                    {
                        //write to buffer
                        stream.Write(audioBuffer, 0, audioBuffer.Length);
                    }
                    //if sound is no longer detected, and is still recording
                    else if (!containsVoice && isRecording)
                    {
                        //save to file
                        wavPath = Path.Combine(audioDir, Guid.NewGuid().ToString() + "_audio.wav");

                        using (System.IO.Stream outputStream = System.IO.File.Open(wavPath, FileMode.Create))
                        using (BinaryWriter bWriter = new BinaryWriter(outputStream))
                        {
                            //write header
                            WriteWaveFileHeader(bWriter, totalAudioLen);

                            memory.WriteTo(outputStream);

                            //close file
                            outputStream.Close();
                            bWriter.Close();

                            isRecording = false;
                        }


                        //this file is now fully written and can be sent to server for analysis
                        AudioFileQueue.Add(new Tuple<DateTime,string>(DateTime.Now, wavPath));
                    }
                    else
                    //no voice
                    {
                        ;
                    }
                }
                //break out of continously loop

                //TODO: handle break

                audioRecord.Stop();
                audioRecord.Dispose();
            }
        }

        private async Task RecordAudioAsync()
        {
            wavPath = Path.Combine(audioDir, Guid.NewGuid().ToString() + "_audio.wav");

            byte[] audioBuffer = new byte[8000];

            audioRecord = new AudioRecord(
                AudioSource.Mic,// Hardware source of recording.
                sampleRate,// Frequency
                channelIn,// Mono or stereo
                encoding,// Audio encoding
                audioBuffer.Length// Length of the audio clip.
            );

            var id = audioRecord.AudioSessionId;

            audioRecord.StartRecording();

            int totalAudioLen = 0;

            isRecording = true;


            using (System.IO.Stream outputStream = System.IO.File.Open(wavPath, FileMode.Create))
            using (BinaryWriter bWriter = new BinaryWriter(outputStream))
            {
                //init a header with no length - it will be added later
                WriteWaveFileHeader(bWriter, maxAudioFreamesLength);

                /// Keep reading the buffer while there is audio input.
                while (isRecording && totalAudioLen <= maxAudioFreamesLength)
                {
                    totalAudioLen += await audioRecord.ReadAsync(audioBuffer, 0, audioBuffer.Length);
                    bWriter.Write(audioBuffer);
                    
                    //analysis
                    var intbuffer = ByteArrayTo16Bit(audioBuffer);
                    var min = intbuffer.Min();
                    var max = intbuffer.Max();
                    var avg = intbuffer.Average(x => (double)x);
                    var sos = intbuffer.Select(x => (long)x)
                        .Aggregate((prev, next) => prev + next * next);
                    var rms = Math.Sqrt((double)1/intbuffer.Length * sos);
                    var fft = FFT(intbuffer);
                }

                isRecording = false;

                //update header length
                //WriteWaveFileHeader(bWriter, totalAudioLen);

                //write lenght to header
                outputStream.Close();
                bWriter.Close();
            }

            audioRecord.Stop();
            audioRecord.Dispose();

            //this file is now fully written and can be sent to server for analysis
            AudioFileQueue.Add(new Tuple<DateTime, string>(DateTime.Now, wavPath));
        }


        void PlayAudioTrack(byte[] audBuffer)
        {
            audioTrack = new AudioTrack(
                Android.Media.Stream.Music,// Stream type
                sampleRate,// Frequency
                channelOut,// Mono or stereo
                encoding,// Audio encoding
                audBuffer.Length,// Length of the audio clip.
                AudioTrackMode.Stream// Mode. Stream or static.
            );

            audioTrack.Play();
            audioTrack.Write(audBuffer, 0, audBuffer.Length);
        }
    }
}

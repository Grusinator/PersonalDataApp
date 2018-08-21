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
            sampleRateInHz: sampleFrequency, 
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

        public double ThresholdValue { get; set; } = 0;


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
            try
            {
                byte[] fileData = File.ReadAllBytes(wavPath);
                new Thread(delegate ()
                {
                    PlayAudioTrack(fileData);
                }).Start();
            }
            catch
            {

            }
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
            Task.Run(async () => await RecordAudioContinuously());
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


        private async Task RecordAudioContinuously()
        {
            byte[] audioBuffer = new byte[8000];
            byte[] preAudioBuffer = new byte[8000];

            audioRecord = new AudioRecord(
                AudioSource.Mic,// Hardware source of recording.
                sampleFrequency,// Frequency
                channelIn,// Mono or stereo
                encoding,// Audio encoding
                audioBuffer.Length// Length of the audio clip.
            );

            _forceStop = false;

            audioRecord.StartRecording();

            using (MemoryStream memory = new MemoryStream())
            using (BufferedStream stream = new BufferedStream(memory))
            {
                while (!_forceStop)
                {
                    //start listening
                    await audioRecord.ReadAsync(audioBuffer, 0, audioBuffer.Length);

                    //analysis
                    var intbuffer = ByteArrayTo16Bit(audioBuffer);

                    var audioData = new AudioData(intbuffer, sampleFrequency, isRecording);

                    audioData.FftPeakFrequency();

                    if (audioData.IsAllZeros)
                    {
                        //not sure if it is neccesary
                        isRecording = false;
                        memory.Flush();
                        memory.Clear(); // this one is though
                        continue;
                    };

                    //this should be smarter ;)
                    containsVoice = audioData.IdentifyVoice(ThresholdValue);

                    //send info to MVVM to display
                    OnRecordStatusChanged(new AudioDataEventArgs(audioData));


                    //if voice has been detected, start writing 
                    if (containsVoice && !isRecording)
                    {
                        isRecording = true;
                        stream.Write(preAudioBuffer, 0, preAudioBuffer.Length);
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

                        //how much audio do we have
                        if ((int)memory.Length <= 2 * audioBuffer.Length)
                        {
                            //this is probably a false positive, at least no valid sound because to short
                            isRecording = false;
                            continue;
                        }
                        else
                        {
                            //Get one more segment of sound
                            await audioRecord.ReadAsync(audioBuffer, 0, audioBuffer.Length);
                            stream.Write(audioBuffer, 0, audioBuffer.Length);

                            using (System.IO.Stream outputStream = System.IO.File.Open(wavPath, FileMode.Create))
                            using (BinaryWriter bWriter = new BinaryWriter(outputStream))
                            {
                                //write header
                                WriteWaveFileHeader(bWriter, (int)memory.Length);

                                memory.WriteTo(outputStream);

                                //close file
                                outputStream.Close();
                                bWriter.Close();

                                isRecording = false;
                            }

                            OnAudioReadyForUpload(new AudioUploadEventArgs(DateTime.Now.ToUniversalTime(), wavPath));
                        }
                        //not sure if it is neccesary
                        memory.Flush();
                        memory.Clear(); // this one is though
                    }
                    //no voice
                    else { ; }

                    preAudioBuffer = (byte[])audioBuffer.Clone();
                }
                //break out of continously loop

                //TODO: handle break - does not care if we were recording

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
                sampleFrequency,// Frequency
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
                }

                isRecording = false;

                //write lenght to header
                outputStream.Close();
                bWriter.Close();
            }

            audioRecord.Stop();
            audioRecord.Dispose();

            //this file is now fully written and can be sent to server for analysis
            OnAudioReadyForUpload(new AudioUploadEventArgs(DateTime.Now.ToUniversalTime(), wavPath));
        }


        void PlayAudioTrack(byte[] audBuffer)
        {
            audioTrack = new AudioTrack(
                Android.Media.Stream.Music,// Stream type
                sampleFrequency,// Frequency
                channelOut,// Mono or stereo
                encoding,// Audio encoding
                audBuffer.Length,// Length of the audio clip.
                AudioTrackMode.Stream// Mode. Stream or static.
            );

            audioTrack.Play();
            audioTrack.Write(audBuffer, 0, audBuffer.Length);
        }
    }

    public static class MemoryExtension
    {
        public static void Clear(this MemoryStream source)
        {
            byte[] buffer = source.GetBuffer();
            Array.Clear(buffer, 0, buffer.Length);
            source.Position = 0;
            source.SetLength(0);
        }
    }

}

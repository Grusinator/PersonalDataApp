﻿using System;
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

namespace PersonalDataApp.Droid
{
    public class AudioRecorder : AudioRecorderGeneric, IAudioRecorder
    {
        static Android.Media.Encoding encoding = Android.Media.Encoding.Pcm16bit;
        static ChannelIn channels = ChannelIn.Mono;


        static int minbufferSize = AudioRecord.GetMinBufferSize(
            sampleRateInHz: sampleRate, 
            channelConfig: channels, 
            audioFormat: encoding
        );

        static int maxAudioFreamesLength = 64000;


        public byte[] audiobuffer;
        public string wavPath;
        public bool _is_recording;
        public bool _contains_voice;


        string pathSave { get; set; }
        MediaRecorder mediaRecorder;
        AudioRecord audioRecord;
        MediaPlayer mediaPlayer;


        public AudioRecorder(): base()
        {
            graphqlHandler = new GraphqlHandler();
        }


        private void SetupAudioRecord()
        {

            audioRecord = new AudioRecord(
                AudioSource.Mic, 
                sampleRate, 
                channels, 
                encoding,
                3* minbufferSize);

            //visualizer = new Visualizer(audioRecord.AudioSessionId);

            var bufsize = audioRecord.BufferSizeInFrames/(double)sampleRate;

            audiobuffer = new byte[3 * minbufferSize];

        }

        private void SetupMediaRecorder()
        {
            pathSave = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath.ToString()
                + "/" + new Guid().ToString() + "_audio.3gp";

            mediaRecorder = new MediaRecorder();
            mediaRecorder.SetAudioSource(AudioSource.Mic);
            mediaRecorder.SetOutputFormat(OutputFormat.ThreeGpp);
            mediaRecorder.SetAudioEncoder(AudioEncoder.AmrNb);
            mediaRecorder.SetOutputFile(pathSave);
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
            if (mediaPlayer != null)
            {
                mediaPlayer.Stop();
                mediaPlayer.Release();
            }
        }

        public void StartRecording()
        {
            Task.Run(async () => await ReadAudioAsync());
        }

        public string StopRecording()
        {
            _is_recording = false;
            return wavPath;
        }

        private async Task ReadAudioAsync()
        {
            wavPath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath.ToString()
                + "/" + new Guid().ToString() + "_audio.wav";

            byte[] audioBuffer = new byte[8000];

            audioRecord = new AudioRecord(
                // Hardware source of recording.
                AudioSource.Mic,
                // Frequency
                sampleRate,
                // Mono or stereo
                channels,
                // Audio encoding
                encoding,
                // Length of the audio clip.
                audioBuffer.Length
            );
            var id = audioRecord.AudioSessionId;

            audioRecord.StartRecording();

            int totalAudioLen = 0;
            int totalDataLen;

            _is_recording = true;


            using (System.IO.Stream outputStream = System.IO.File.Open(wavPath, FileMode.Create))
            using (BinaryWriter bWriter = new BinaryWriter(outputStream))
            {
                //init a header with no length - it will be added later
                WriteWaveFileHeader(
                    bWriter,
                    0,
                    0);

                /// Keep reading the buffer while there is audio input.
                while (_is_recording && totalAudioLen <= maxAudioFreamesLength)
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

                //correction for the header, im not sure why 36 - would have expected 44
                totalDataLen = totalAudioLen + 36;

                //update header length
                WriteWaveFileHeader(
                    bWriter,
                    totalAudioLen,
                    totalDataLen);

                //write lenght to header
                outputStream.Close();
                bWriter.Close();
            }

            audioRecord.Stop();
            audioRecord.Dispose();
        }

        private Int16[] ByteArrayTo16Bit(byte[] byteArray)
        {
            int intlength = byteArray.Length/2;

            Int16[] output = new Int16[intlength];
            
            for (int i = 0; i < byteArray.Length; i = i + 2)
            {
                var index = (int)i / 2;
                output[index] = BitConverter.ToInt16(byteArray, i);
            }
            return output;
        }

        public double[] FFT(Int16[] sound)
        {


            Complex[] complexInput = new Complex[sound.Length];
            for (int i = 0; i < complexInput.Length; i++)
            {
                Complex tmp = new Complex(sound[i], 0);
                complexInput[i] = tmp;
            }

            MathNet.Numerics.IntegralTransforms.Fourier.Forward(complexInput);

            return complexInput.ToList().Select(x => x.Magnitude).ToArray();
        }

        public double[] FFT(double[] sound)
        {
            

            Complex[] complexInput = new Complex[sound.Length];
            for (int i = 0; i < complexInput.Length; i++)
            {
                Complex tmp = new Complex(sound[i], 0);
                complexInput[i] = tmp;
            }

            MathNet.Numerics.IntegralTransforms.Fourier.Forward(complexInput);

            return complexInput.ToList().Select(x => x.Magnitude).ToArray();
        }

        void PlayAudioTrack(byte[] audBuffer)
        {
            var audioTrack = new AudioTrack(
                // Stream type
                Android.Media.Stream.Music,
                // Frequency
                sampleRate,
                // Mono or stereo
                ChannelOut.Mono,
                // Audio encoding
                encoding,
                // Length of the audio clip.
                audBuffer.Length,
                // Mode. Stream or static.
                AudioTrackMode.Stream);

            audioTrack.Play();
            audioTrack.Write(audBuffer, 0, audBuffer.Length);
        }
    }
}

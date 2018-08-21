﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using PersonalDataApp.Services;
using System.Numerics;

namespace PersonalDataApp.Models
{
    public class AudioData
    {
        public double Rms { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
        public double Avg { get; set; }
        public double Sos { get; set; }
        public double[] Fft { get; set; }
        public bool? ContainsVoice { get; set; }
        public bool? IsRecording { get; set; }
        public int SampleFrequency { get; set; }
        public bool IsAllZeros { get; set; }
        public double FrequencyPeak { get; set; }
        public double FFT_VoicePower { get; set; }

        static readonly double MinFrequencyCutoff = 80;
        static readonly double MaxFrequencyCutoff = 300;

        public AudioData(short[] intbuffer, int sampleFrequency, bool? isRecording=null)
        {
            SampleFrequency = sampleFrequency;

            Min = intbuffer.Min();
            Max = intbuffer.Max();
            Avg = intbuffer.Average(x => (double)x);
            Sos = intbuffer.Select(x => (long)x).Aggregate((prev, next) => prev + next * next);
            Rms = Math.Sqrt((double)1 / intbuffer.Length * Sos);
            Fft = FFT(intbuffer);

            IsAllZeros = !intbuffer.Any((val) => val != 0);

            IsRecording = isRecording;
        }

        public bool IdentifyVoice(double threshold = 300)
        {
            FFT_VoicePower = Math.Log10(FftAreaWithinVoiceFrequencyBand(MinFrequencyCutoff, MaxFrequencyCutoff));
            ContainsVoice = FFT_VoicePower > threshold;
            return ContainsVoice ?? false;
        }

        public double FftPeakFrequency()
        {
            double maxValue = Fft.Take(Fft.Length/2).Max();
            int maxIndex = Array.IndexOf(Fft, maxValue);
            double frequencyStep = SampleFrequency / Fft.Length;
            FrequencyPeak = maxIndex * frequencyStep;

            return FrequencyPeak;
        }

        public double FftAreaWithinVoiceFrequencyBand(double min=0, double max=double.PositiveInfinity)
        {
            double frequencyStep = SampleFrequency / Fft.Length;

            //limit max frequency to what is the limit of the data
            max = max < SampleFrequency / 2 ? max : SampleFrequency / 2;

            //determine the frequency band to integrate over
            int startIndex = (int)Math.Floor(min / frequencyStep);
            int stopIndex = (int)Math.Ceiling(max / frequencyStep);

            double integral = 0;

            //power is the assumption that the input signal is in voltage, convert to power by squaring
            // maybe missing * 1/2 maybe i should check up on this, but it is not very important
            var FftPow = Fft.Select(x => 0.5*Math.Pow(x, 2)).ToList();

            //Trapez area integration of FFT spectrum
            for (int i = startIndex; i < stopIndex; i++)
            {
                integral += 0.5 * (FftPow[i] + FftPow[i - 1]) * frequencyStep;
            }
            return integral;
        }

        public static double[] FFT(Int16[] sound)
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

        public static double[] FFT(double[] sound)
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
    }
}


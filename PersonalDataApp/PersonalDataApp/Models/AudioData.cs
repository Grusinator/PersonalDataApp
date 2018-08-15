using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using PersonalDataApp.Services;

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

        public AudioData(short[] intbuffer, bool? isRecording=null)
        {
            Min = intbuffer.Min();
            Max = intbuffer.Max();
            Avg = intbuffer.Average(x => (double)x);
            Sos = intbuffer.Select(x => (long)x).Aggregate((prev, next) => prev + next * next);
            Rms = Math.Sqrt((double)1 / intbuffer.Length * Sos);
            Fft = AudioRecorderGeneric.FFT(intbuffer);

            IsAllZeros = !intbuffer.Any((val) => val != 0);

            IsRecording = isRecording;
        }

        internal bool IdentifyVoice(double threshold = 300)
        {
            ContainsVoice = Rms > threshold;
            return ContainsVoice ?? false;
        }
    }
}


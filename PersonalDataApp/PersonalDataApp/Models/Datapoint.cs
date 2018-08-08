using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalDataApp.Models
{
    public class Datapoint
    {
        public int Id { get; set; }
        public DateTime Datetime { get; set; }
        public string Category { get; set; }
        public string SourceDevice { get; set; }
        public double Value { get; set; }
        public string TextFromAudio { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalDataApp.Models
{
    public class Datapoint
    {
        public DateTime datetime { get; set; }
        public string category { get; set; }
        public string source_device { get; set; }
        public double value { get; set; }
        public string text_from_audio { get; set; }
    }
}

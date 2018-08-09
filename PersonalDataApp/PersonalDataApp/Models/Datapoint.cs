using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using PersonalDataApp.Services;

namespace PersonalDataApp.Models
{
    public class Datapoint
    {
        [JsonIgnore]
        public int Id { get; set; }

        public DateTime Datetime { get; set; }
        public string Category { get; set; }
        public string SourceDevice { get; set; }
        public double Value { get; set; }
        public string TextFromAudio { get; set; }
    }
}

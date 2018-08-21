using System;
using System.Collections.Generic;
using System.Text;
using Prism.Events;

namespace PersonalDataApp.Models
{
    public class IsThresholdUpdated : PubSubEvent<Double> { }
}

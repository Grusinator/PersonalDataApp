﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using PersonalDataApp.Models;
using Prism.Events;
using Xamarin.Forms;

namespace PersonalDataApp.Models
{
    public class IsThresholdUpdated : PubSubEvent<Double> { }

    public class ItemSelectedEventArgs : EventArgs
    {
        public Datapoint Datapoint { get; }
    }

    public class ItemSelectedEventArgsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var itemSelectedEventArgs = value as ItemSelectedEventArgs;
            if (itemSelectedEventArgs == null)
            {
                throw new ArgumentException("Expected value to be of type ItemTappedEventArgs", nameof(value));
            }
            return itemSelectedEventArgs.Datapoint;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    } 
}

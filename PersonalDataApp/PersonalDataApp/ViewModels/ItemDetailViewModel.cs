using System;

using PersonalDataApp.Models;

namespace PersonalDataApp.ViewModels
{
    public class ItemDetailViewModel : BaseViewModel
    {
        public Datapoint Datapoint { get; set; }
        public ItemDetailViewModel(Datapoint datapoint = null)
        {
            Title = datapoint?.Category;
            Datapoint = datapoint;
        }
    }
}

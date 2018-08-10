using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using PersonalDataApp.Models;
using PersonalDataApp.Views;

namespace PersonalDataApp.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {

        string errorMessage = String.Empty;
        public string ErrorMessage
        {
            get { return errorMessage; }
            set { SetProperty(ref errorMessage, value); }
        }

        


    }
}

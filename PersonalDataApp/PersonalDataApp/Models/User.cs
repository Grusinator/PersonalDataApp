using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalDataApp.Models
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public string Name { get; set; }

        public DateTime Birthdate { get; set; }

        public string Language { get; set; }
        public double AudioThreshold { get; set; }


    }
}

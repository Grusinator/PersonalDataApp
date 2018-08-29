using PersonalDataApp.Authentication;
using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalDataApp.Services
{
    public interface IIntentHandler
    {
        GoogleAuthenticator Auth { get; set; }
        void StartIntent(GoogleAuthenticator Auth);
    }


}

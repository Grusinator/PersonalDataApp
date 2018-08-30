using System;
using PersonalDataApp.Services.Authorization;

namespace PersonalDataApp.Authentication
{
    public interface IAuthenticationDelegate
    {
        void OnAuthenticationCompleted(OAuthToken token);
        void OnAuthenticationFailed(string message, Exception exception);
        void OnAuthenticationCanceled();
    }
}

using System;

namespace PersonalDataApp.Authentication
{
    public interface IGoogleAuthenticationDelegate
    {
        void OnAuthenticationCompleted(GoogleOAuthToken token);
        void OnAuthenticationFailed(string message, Exception exception);
        void OnAuthenticationCanceled();
    }
}

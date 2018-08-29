namespace PersonalDataApp.Authentication
{
    public static class Configuration
    {

        //google 
        //public const string AuthorizeUrl = "https://accounts.google.com/o/oauth2/v2/auth";
        //public const string AccessTokenUrl = "https://www.googleapis.com/oauth2/v4/token";
        //private const string ClientSecret = string.Empty;

        //public const string ClientId = "1074677609523-1norcne8e6f9lgi74b3k27c7cf7oe5va.apps.googleusercontent.com";
        //public const string Scope = "email";

        //nokia
        //public const string AuthorizeUrl = "https://account.health.nokia.com/oauth2_user/authorize2";
        //public const string AccessTokenUrl = "https://account.health.nokia.com/oauth2/token";
        //public const string ClientSecret = "1f1d852451385469a56ef6494cbd2e94c07421c3ee5ffbfca63216079fd36d1a";

        //public const string ClientId = "a80378abe1059ef7c415cf79b09b1270f828c4a0fbfdc52dbec06ae5f71b4bb6";
        //public const string Scope = "user.info";
        //public const string RedirectUrl = "com.wshconsulting.personaldataapp:/oauth2redirect";

        //strava
        public const string AuthorizeUrl = "https://www.strava.com/oauth/authorize";
        public const string AccessTokenUrl = "https://www.strava.com/oauth/token";
        public const string ClientId = "28148";
        public const string Scope= "view_private";
        public const string ClientSecret = "ed5f469f798830c7214fc8efad54790799fc3ae1";
        //public const string RedirectUrl = "com.wshconsulting.personaldataapp:/oauth2redirect";
        //public const string RedirectUrl = "http://oauthdebugger.com/debug";
        public const string RedirectUrl = "http://localhost:/oauth2redirect";


    }
}
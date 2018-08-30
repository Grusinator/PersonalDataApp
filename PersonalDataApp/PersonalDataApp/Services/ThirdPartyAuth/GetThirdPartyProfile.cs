using Newtonsoft.Json;
using PersonalDataApp.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;


namespace PersonalDataApp.Services.Authorization
{
    public class GetThirdPartyProfile
    {
        public static async Task<string> GetProfileAsync(ThirdPartyDataProvider dataProvider)
        {
            if (string.IsNullOrWhiteSpace(dataProvider.ProfileRequestUrl))
            { return null; }

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization 
                = new AuthenticationHeaderValue(
                    dataProvider.AccessToken.TokenType,
                    dataProvider.AccessToken.AccessToken
                    );
            var json = await httpClient.GetStringAsync($"{dataProvider.ProfileRequestUrl}?alt=json");
            return json;
        }
    }
}

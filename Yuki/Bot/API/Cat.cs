using System.Net.Http;
using Yuki.Bot.Services;

namespace Yuki.Bot.API
{
    public class Cat
    {
        public static string GetImage()
        {
            using (HttpClient http = new HttpClient())
                return http.GetAsync("http://thecatapi.com/api/images/get?format=src&api_key=" + YukiClient.Instance.Credentials.CatApiKey).Result.RequestMessage.RequestUri.AbsoluteUri;
        }
    }
}

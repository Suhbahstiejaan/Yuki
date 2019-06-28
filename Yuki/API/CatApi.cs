using System.Net.Http;
using System.Threading.Tasks;
using Yuki.Data;

namespace Yuki.API
{
    public static class CatApi
    {
        public static async Task<string> GetImage()
        {
            using (HttpClient http = new HttpClient())
            {
                return (await http.GetAsync($"http://thecatapi.com/api/images/get?format=src&api_key={Config.GetConfig().cat_api}")).RequestMessage.RequestUri.AbsoluteUri;
            }
        }
    }
}

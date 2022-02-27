using System.Net.Http;
using System.Net.Http.Headers;

namespace ProActiveBot.Bot.Helpers
{
    public static class HTTPRequestHelper
    {
        public static string Get(string url)
        {
            HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsStringAsync().Result;
            }
            else
            {
                return null;
            }
        }


        public static string Post(string url, HttpContent httpContent)
        {
            HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.PostAsync(url, httpContent).Result;
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsStringAsync().Result;
            }
            else
            {
                return null;
            }
        }
    }
}

using System;
using System.Net;
using System.Net.Http;
using System.Text;

namespace PEMTester
{
    class HttpSender
    {
       private readonly HttpClient client;


        private static HttpSender instance;

        private HttpSender()
        {
            client = new HttpClient();
        }

        public Action<int> OnPostComplete;

        public static HttpSender Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new HttpSender();
                }

                return instance;
            }
        }

        internal async void Post(string jsonValue, string url)
        {
            try
            {

                HttpContent content = new StringContent(jsonValue, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, content);

                var responseString = await response.Content.ReadAsStringAsync();

                var s = string.Empty;
                s = responseString;

                OnPostComplete?.Invoke((int)response.StatusCode);
            }
            catch
            {
                OnPostComplete?.Invoke(400);
            }


            

        }
        
    }
}

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

        public Action<int, string> OnPostComplete;

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

                OnPostComplete?.Invoke((int)response.StatusCode, response.ReasonPhrase);

            }
            catch
            {
                OnPostComplete?.Invoke(400, string.Empty);
            }

            

        }
        
    }
}

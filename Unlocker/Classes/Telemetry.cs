using System;
using System.Collections.Generic;
using System.Net.Http;

namespace FortniteBurger.Classes
{
    internal class Telemetry
    {
        private static HttpClient ApiClient = new()
        {
            BaseAddress = new Uri("https://api.fortniteburger.vip/")
        };

        internal static void Load()
        {
            ApiClient.DefaultRequestHeaders.UserAgent.ParseAdd("burger");
        }

        internal async static void Add()
        {
            try
            {
                using HttpResponseMessage response = await ApiClient.PostAsync(
                    "counter",
                    new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("type", "add"),
                        new KeyValuePair<string, string>("guid", GUID.GetGUID()),
                    })
                );
            } catch
            {

            }
        }
    }
}

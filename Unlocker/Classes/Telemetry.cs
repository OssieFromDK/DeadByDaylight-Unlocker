using System;
using System.Collections.Generic;
using System.Net.Http;

namespace FortniteBurger.Classes
{
    internal class Telemetry
    {
        private static HttpClient ApiClient = new()
        {
            BaseAddress = new Uri("https://burger.ossie.dk/api/")
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
            } catch(Exception e)
            {
                MainWindow.ErrorLog.CreateLog(e.Message);
            }
        }

        internal async static void Remove()
        {
            try
            {
                using HttpResponseMessage response = await ApiClient.PostAsync(
                    "counter",
                    new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("type", "remove"),
                        new KeyValuePair<string, string>("guid", GUID.GetGUID()),
                    })
                );
            }
            catch (Exception e)
            {
                MainWindow.ErrorLog.CreateLog(e.Message);
            }
        }
    }
}

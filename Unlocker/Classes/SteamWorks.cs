using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FortniteBurger.Classes
{
    internal static class SteamWorks
    {
        public static string BHVRCookie = null;

        public static async void GetBhvrSession()
        {
            if (!SteamAPI.Init())
            {
                MainWindow.cookie.ReturnFromCookie("Steam must be running", false);
                return;
            }

            byte[] pTicket = new byte[1024];
            SteamUser.GetAuthSessionTicket(pTicket, 1024, out uint _);
            SteamAPI.Shutdown();
            string ticket = BitConverter.ToString(pTicket).Replace("-", string.Empty).TrimEnd('0', ' ');
            if (ticket == null)
            {
                MainWindow.cookie.ReturnFromCookie("Ticket was null, try again", false);
                return;
            }

            using (HttpClient client = new HttpClient())
            {
                StringContent bodyContent = new StringContent("{\"clientData\":{}}");
                bodyContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://steam.live.bhvrdbd.com/api/v1/auth/provider/steam/login?token=" + ticket);
                request.Headers.Add("Accept", "*/*");
                request.Headers.Add("Accept-Encoding", "deflate, gzip");
                request.Headers.Add("x-kraken-client-platform", "egs");
                request.Headers.Add("x-kraken-client-provider", "egs");
                request.Headers.Add("x-kraken-client-resolution", "1920x1080");
                request.Headers.Add("x-kraken-client-timezone-offset", "-60");
                request.Headers.Add("x-kraken-client-os", "10.0.22621.1.256.64bit");
                request.Headers.Add("User-Agent", "DeadByDaylight/++DeadByDaylight+Live-CL-923411 EGS/10.0.22621.1.256.64bit");
                request.Content = (HttpContent)bodyContent;
                HttpResponseMessage response = await client.SendAsync(request);
                IEnumerable<string> sessionValues;
                if (response.Headers.TryGetValues("Set-Cookie", out sessionValues))
                {
                    foreach (string sessionValue in sessionValues)
                    {
                        int index = sessionValue.IndexOf(";");
                        string session = sessionValue.Substring(0, index);
                        CookieHandler.SetCookie(session);
                        MainWindow.cookie.ReturnFromCookie("Successfully Grabbed Cookie", true);
                        session = null;
                    }
                }
                bodyContent = null;
                request = null;
                response = null;
                sessionValues = null;
            }
            pTicket = null;
            ticket = null;
        }
    }
}

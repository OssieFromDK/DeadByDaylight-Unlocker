using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortniteBurger.Classes
{
    internal static class CookieHandler
    {
        private static string BHVRCookie = null;

        internal static void ResetCookie()
        {
            BHVRCookie = null;
        }

        internal static void SetCookie(string cookie)
        {
            if (string.IsNullOrEmpty(cookie)) return;

            BHVRCookie = cookie;
        }

        internal static string GetCookie()
        {
            return BHVRCookie;
        }
    }
}

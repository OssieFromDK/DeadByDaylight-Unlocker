using Fiddler;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Windows;

namespace FortniteBurger.Classes
{
    internal class FiddlerCore
    {
        public static bool FiddlerIsRunning = false;
        internal static SessionStateHandler GrabWithShutdown = new SessionStateHandler(CookieGrabWithShutdown);
        internal static SessionStateHandler GrabWithoutShutdown = new SessionStateHandler(CookieGrabWithoutShutdown);
        internal static SessionStateHandler LaunchedWithProfileEditor = new SessionStateHandler(ProfileEditor);
        internal static string MyPlayerId = string.Empty;

        private static void EnsureRootCertGrabber()
        {
            CertMaker.createRootCert();
            string str = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FortniteBurger");
            if (!Directory.Exists(str))
                Directory.CreateDirectory(str);
            string path = Path.Combine(str, "root.cer");
            X509Certificate2 rootCertificate = CertMaker.GetRootCertificate();
            rootCertificate.FriendlyName = "FortniteBurger";
            File.WriteAllBytes(path, rootCertificate.Export(X509ContentType.Cert));
            X509Store x509Store = new X509Store(StoreName.Root, StoreLocation.CurrentUser);
            x509Store.Open(OpenFlags.ReadWrite);
            x509Store.Add(rootCertificate);
            x509Store.Close();
        }

        public static void StartFiddlerCore()
        {
            FiddlerApplication.Prefs.SetStringPref("fiddler.certmaker.bc.RootCN", "FortniteBurger");
            FiddlerApplication.Prefs.SetStringPref("fiddler.certmaker.bc.RootFriendly", "FortniteBurger");
            EnsureRootCertGrabber();
            //EnsureRootCertificate();
            FiddlerIsRunning = true;
            CONFIG.IgnoreServerCertErrors = true;
            CONFIG.EnableIPv6 = true;
            Random random = new Random();
            int port = random.Next(7777, 9999);
            FiddlerApplication.Startup(new FiddlerCoreStartupSettingsBuilder().ListenOnPort((ushort)port).DecryptSSL().RegisterAsSystemProxy().Build());
        }

        public static void StartWithShutdown()
        {
            FiddlerApplication.BeforeRequest += GrabWithShutdown;
        }

        public static void StartWithoutShutdown()
        {
            FiddlerApplication.BeforeRequest -= GrabWithoutShutdown;
            FiddlerApplication.BeforeRequest += GrabWithoutShutdown;
        }

        public static void LaunchProfileEditor()
        {
            FiddlerApplication.BeforeRequest -= LaunchedWithProfileEditor;
            FiddlerApplication.BeforeRequest += LaunchedWithProfileEditor;
            FiddlerApplication.BeforeResponse += FiddlerApplication_BeforeRespone;
        }

        public static void StopFiddlerCore()
        {
            CertMaker.removeFiddlerGeneratedCerts(true);

            FiddlerApplication.BeforeRequest -= LaunchedWithProfileEditor;
            FiddlerApplication.BeforeRequest -= GrabWithoutShutdown;
            FiddlerApplication.BeforeResponse -= FiddlerApplication_BeforeRespone;

            FiddlerApplication.Shutdown();

            FiddlerIsRunning = false;
        }

        private static void ProfileEditor(Session oSession)
        {
            if (oSession.uriContains("/api/v1/dbd-character-data/get-all") && MainWindow.profile.FullProfile && !MainWindow.profile.Off)
                oSession.oFlags["x-replywithfile"] = Settings.ProfilePath + "/Profile.json";

            if (oSession.uriContains("/api/v1/dbd-character-data/bloodweb") && MainWindow.profile.FullProfile && !MainWindow.profile.Off)
                oSession.oFlags["x-replywithfile"] = Settings.ProfilePath + "/Bloodweb.json";

            if (oSession.uriContains("/api/v1/dbd-inventories/all") && !MainWindow.profile.Off && !oSession.uriContains("consume"))
            {
                if (MainWindow.profile.FullProfile)
                {
                    oSession.oFlags["x-replywithfile"] = Settings.ProfilePath + "/SkinsWithItems.json";
                }
                else if(MainWindow.profile.Skins_Only)
                {
                    oSession.oFlags["x-replywithfile"] = Settings.ProfilePath + "/SkinsONLY.json";
                }
                else if(MainWindow.profile.Skins_Perks_Only)
                {
                    oSession.oFlags["x-replywithfile"] = Settings.ProfilePath + "/SkinsPerks.json";
                }
                else if (MainWindow.profile.DLC_Only)
                {
                    oSession.oFlags["x-replywithfile"] = Settings.ProfilePath + "/DlcOnly.json";
                }
            }

            if (oSession.uriContains("api/v1/wallet/currencies") && MainWindow.profile.Currency_Spoof)
                oSession.oFlags["x-replywithfile"] = Settings.ProfilePath + "/Currency.json";

            if ((oSession.uriContains("api/v1/extensions/playerLevels/getPlayerLevel")  || oSession.uriContains("api/v1/extensions/playerLevels/earnPlayerXp")) && MainWindow.profile.Level_Spoof)
                oSession.oFlags["x-replywithfile"] = Settings.ProfilePath + "/Level.json";

            //if (oSession.uriContains("/catalog") && (MainWindow.profile.Break_Skins) && !MainWindow.profile.Off)
            //oSession.oFlags["x-replywithfile"] = Settings.ProfilePath + "/Catalog.json";

            //if (oSession.uriContains("itemsKillswitch") && (MainWindow.profile.Disabled) && !MainWindow.profile.Off)
            //oSession.oFlags["x-replywithfile"] = Settings.ProfilePath + "/Disabled.json";
        }

        private static void FiddlerApplication_BeforeRespone(Session oSession)
        {
            if (oSession.uriContains("api/v1/auth/provider/"))
            {
                oSession.utilDecodeResponse();
                string Response = oSession.GetResponseBodyAsString();
                JObject JSON = JsonConvert.DeserializeObject<JObject>(Response);

                MyPlayerId = JSON["userId"].ToString();
                Utils.CID(MyPlayerId);
                MainWindow.settings.UpdatePlayerId(MyPlayerId);
            }
        }


        private static void CookieGrabWithoutShutdown(Session oSession)
        {
            if (oSession.uriContains("api/v1/config"))
            {
                if (oSession.oRequest["Cookie"].Length > 0)
                {
                    CookieHandler.SetCookie(oSession.oRequest["Cookie"]);
                }
            }
        }

        private static void CookieGrabWithShutdown(Session oSession)
        {
            if (oSession.uriContains("api/v1/config"))
            {
                if (oSession.oRequest["Cookie"].Length > 0)
                {
                    CookieHandler.SetCookie(oSession.oRequest["Cookie"]);
                    MainWindow.cookie.ReturnFromCookie("Successfully Grabbed Cookie", true);
                    FiddlerApplication.BeforeRequest -= GrabWithShutdown;
                    StopFiddlerCore();
                }
            }
        }

    }
}

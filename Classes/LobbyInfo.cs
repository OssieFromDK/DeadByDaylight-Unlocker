using Fiddler;
using Newtonsoft.Json.Linq;
using Steamworks;
using System;
using System.IO;

namespace FortniteBurger.Classes
{
    internal class LobbyInfo
    {
        public EventHandler<(string ETA, string POS)> QueueUpdated;
        public EventHandler<(string Rank, string Country, string Rating, string Server, string Killer)> MatchUpdated;
        public EventHandler<(string MatchId, string Platform, string SteamId)> MatchInfoUpdated;
        public EventHandler QueueCancelled;

        internal LobbyInfo()
        {
            EnsureRootCertLobby();
        }

        internal void Start()
        {
            CONFIG.IgnoreServerCertErrors = true;
            FiddlerApplication.BeforeResponse += new SessionStateHandler(this.FiddlerBeforeResponse);
        }

        internal void Stop()
        {
            FiddlerApplication.BeforeResponse -= new SessionStateHandler(FiddlerBeforeResponse);
        }

        private void FiddlerBeforeResponse(Session oSession)
        {
            if (oSession.uriContains("/api/v1/queue"))
            {
                if (!oSession.uriContains("/cancel"))
                {
                    if (oSession.uriContains("token/issue"))
                    {
                        return;
                    }

                    // In Queue

                    oSession.bBufferResponse = true;
                    oSession.utilDecodeResponse();
                    string responseBodyAsString = oSession.GetResponseBodyAsString();
                    if (string.IsNullOrEmpty(responseBodyAsString))
                    {
                        return;
                    }

                    JObject ResponeParsedObject = JObject.Parse(responseBodyAsString);
                    JToken ResponeParsedToken_Status = ResponeParsedObject.SelectToken("status");
                    if (ResponeParsedToken_Status.ToString().Equals("QUEUED"))
                    {
                        oSession.bBufferResponse = true;
                        oSession.utilDecodeResponse();
                        // Queued
                        JToken ResponeParsedToken_QueueDataETA = ResponeParsedObject.SelectToken("queueData.ETA");
                        JToken ResponeParsedToken_QueueDataPOS = ResponeParsedObject.SelectToken("queueData.position");
                        ProcessQueue(
                            ResponeParsedToken_QueueDataETA?.ToString(),
                            ResponeParsedToken_QueueDataPOS?.ToString()
                        );
                    }


                    if (ResponeParsedToken_Status.ToString().Equals("MATCHED"))
                    {
                        oSession.bBufferResponse = true;
                        oSession.utilDecodeResponse();
                        // Match Found
                        JToken ResponeParsedToken_MatchDataRANK = ResponeParsedObject.SelectToken("matchData.skill.rank");
                        JToken ResponeParsedToken_MatchDataCountry = ResponeParsedObject.SelectToken("matchData.skill.countries[0]");
                        JToken ResponeParsedToken_MatchDataRating = ResponeParsedObject.SelectToken("matchData.skill.rating.rating");
                        JToken ResponeParsedToken_MatchDataCrossplay = ResponeParsedObject.SelectToken("matchData.props.regions.CrossplayOptOut");
                        JToken ResponeParsedToken_MatchDataCharacter = ResponeParsedObject.SelectToken("matchData.props.characterName");
                        ProcessMatchedInfo(
                            ResponeParsedToken_MatchDataRANK?.ToString(),
                            ResponeParsedToken_MatchDataCountry?.ToString(),
                            ResponeParsedToken_MatchDataRating?.ToString(),
                            ResponeParsedToken_MatchDataCrossplay?.ToString(),
                            ResponeParsedToken_MatchDataCharacter?.ToString()
                        );
                    }
                    ResponeParsedToken_Status.ToString().Equals("MATCHED");
                }
                else
                {
                    FireQueueCancelled();
                }
            }
            else if (oSession.uriContains("api/v1/match") && !oSession.GetResponseBodyAsString().Contains("forbidden"))
            {
                // In Match
                oSession.bBufferResponse = true;
                oSession.utilDecodeResponse();
                string responseBodyAsString = oSession.GetResponseBodyAsString();
                JObject ResponeParsedObject = JObject.Parse(responseBodyAsString);


                JToken ResponeParsedToken_MatchID = ResponeParsedObject.SelectToken("matchId");
                JToken ResponeParsedToken_CloudID = ResponeParsedObject.SelectToken("sideA[0]");
                JToken ResponeParsedToken_Platform = ResponeParsedObject.SelectToken("props.platform");

                string matchid = ResponeParsedToken_MatchID?.ToString();
                string plat = ResponeParsedToken_CloudID?.ToString();
                string cloudid = ResponeParsedToken_Platform?.ToString();

                ProcessMatchInfo(matchid, cloudid, plat);
            }
            else if (oSession.uriContains("api/v1/party") || oSession.uriContains("api/v1/party/details"))
            {
                // Party

                oSession.bBufferResponse = true;
                oSession.utilDecodeResponse();
                string responseBodyAsString = oSession.GetResponseBodyAsString();
                if (string.IsNullOrEmpty(responseBodyAsString))
                {
                    return;
                }

                JToken ResponeParsedToken_Party_MatchState = JObject.Parse(responseBodyAsString).SelectToken("gameSpecificState._partyMatchmakingSettings._matchmakingState");

                if (ResponeParsedToken_Party_MatchState == null)
                {
                    FireQueueCancelled();
                }
                else
                {
                    if (!ResponeParsedToken_Party_MatchState.ToString().Equals("None"))
                    {
                        return;
                    }

                    FireQueueCancelled();
                }
            }
        }

        private void EnsureRootCertLobby()
        {
            BCCertMaker.BCCertMaker bcCertMaker = new BCCertMaker.BCCertMaker();
            CertMaker.oCertProvider = bcCertMaker;
            string str = Path.Combine(Path.GetTempPath(), "certificate.p12");
            string password = "kokot";
            if (!File.Exists(str))
            {
                CertMaker.removeFiddlerGeneratedCerts(true);
                bcCertMaker.CreateRootCertificate();
                bcCertMaker.WriteRootCertificateAndPrivateKeyToPkcs12File(str, password, null);
            }
            else
                bcCertMaker.ReadRootCertificateAndPrivateKeyFromPkcs12File(str, password, null);
            if (CertMaker.rootCertIsTrusted())
                return;
            CertMaker.trustRootCert();
        }

        // In Queue
        private void ProcessQueue(string Found_ETA, string Found_POS)
        {
            QueueUpdated?.Invoke(this,
                (ETA: Utils.CalculateETA(Found_ETA).ToString(), POS: Found_POS)
            );
        }

        // Match Found
        private void ProcessMatchedInfo(string Found_Rank, string Found_Country, string Found_Rating, string Found_Server, string Found_Character)
        {
            MatchUpdated?.Invoke(this, 
                (Rank: Found_Rank, Country: Utils.TranslateCountry(Found_Country), Rating: Utils.CalculateMMR(Found_Rank).ToString(), Server: Found_Server, Killer: Utils.TranslateCharacter(Found_Character))
            );
        }

        // Match Found
        private void ProcessMatchInfo(string Found_MatchID, string Found_CloudID, string Found_Platform)
        {
            string[] result = Utils.FindKillerSteam("https://steam.live.bhvrdbd.com/", Found_CloudID, CookieHandler.GetCookie()).Result;
            string Found_Steam = string.IsNullOrEmpty(result[1]) ? "Console Player" : result[1];

            MatchInfoUpdated?.Invoke(this,
                (MatchId: Found_MatchID, Platform: Found_Platform, SteamId: Found_Steam)
            );
        }

        private void FireQueueCancelled()
        {
            QueueCancelled?.Invoke(this, EventArgs.Empty);
        }


    }
}

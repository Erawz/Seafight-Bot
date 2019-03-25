using System;
using System.Collections.Generic;

namespace BoxyBot
{
    public static class BotSession
    {
        public static Dictionary<string, string> userLoginData = new Dictionary<string, string>();
        public static DateTime sessionStartTime;
        public static DateTime dailyQuestFinishTime;
        public static string currentCookies;
        public static string currentUrl;
        public static string htmlpage;
        public static string Username;
        public static string Password;
        public static string Server;
        public static string Sessionid;
        public static string language = "en";
        public static string Userid;
        public static bool loggedIn;
        public static bool logginIn;
        public static string Host;
        public static string IP;
        public static string Port;
        public static string log;
        public static int selectedTaskIndex;
        public static bool lostConnection;
        public static string currentQuest = "";
        public static string krakenName = "Kraken";
        public static string krakenTentacleName = "Kraken Tentacle";

        public static int Sessiongold { get; set; }
        public static int Sessionpearls { get; set; }
        public static int Sessionxp { get; set; }
        public static int Sessionhp { get; set; }
        public static int Sessionmojos { get; set; }
        public static int Sessioncrystals { get; set; }
        public static int Sessioncrowns { get; set; }
        public static int Sessionglitters { get; set; }
        public static int Sessionchests { get; set; }
        public static int Sessionnpcs { get; set; }
        public static int Sessionmonsters { get; set; }
        public static int Sessionbms { get; set; }
        public static int Sessionwaves { get; set; }
        public static int Sessionbmnpcs { get; set; }
        public static int Sessionelps { get; set; }
        public static int Sessionpowder { get; set; }
        public static int Sessionplates { get; set; }
        public static int Sessiondeaths { get; set; }
        public static int Sessionreconnects { get; set; }
        public static int Sessionraidrejoins { get; set; }
        public static int Sessionattackedbyplayer { get; set; }
        public static int Sessionescapedisland { get; set; }
        public static int Sessionshotback { get; set; }
        public static int Sessionescapedplayer { get; set; }
        public static int Sessiondestroyedplayer { get; set; }
        public static int Sessionradiantsouls { get; set; }
        public static int Sessioncursedsouls { get; set; }
        public static int Sessionharpoondamage { get; set; } = -1;
        public static double Sessioncanondamage { get; set; } = -1;
        public static string HuntNPCName { get; set; }
        public static int HuntNPCnameId { get; set; } = -1;
        public static int Canondamage { get; set; }
        public static bool Sessionacceptedloginbonus { get; set; } = false;
        public static bool Sessioncandodailyquest { get; set; } = true;
        public static Dictionary<double, int> attackRetrys = new Dictionary<double, int>();
    }
}

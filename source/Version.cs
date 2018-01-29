using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;
using Newtonsoft.Json.Linq;

namespace Move_It
{
    class Version
    {
        public static void Check(string v, ReadyEventArgs e)
        {
            WebClient n = new WebClient();
            var json = n.DownloadString("http://apis.creeperman007.tk/request-yt/v1/");
            string valueOriginal = Convert.ToString(json);
            JObject data = JObject.Parse(valueOriginal);
            string version = Convert.ToString(data["latestVersion"]);
            if (v == version)
                e.Client.DebugLogger.LogMessage(LogLevel.Info, "Updater", "You have latest version.", DateTime.Now);
            else
                e.Client.DebugLogger.LogMessage(LogLevel.Warning, "Updater", "New version available. Please, consider updating: http://github.com/Creeperman007/Move_It/releases", DateTime.Now);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Move_It
{
    class Credentials
    {
        private ulong user_id;
        private ulong guild_id;
        private string token;

        public Credentials(string path)
        {
            try
            {
                JObject config = JObject.Parse(File.ReadAllText(path));
                user_id = (ulong)config["user"];
                guild_id = (ulong)config["guild"];
                token = (string)config["token"];
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException(String.Format("Config file ({0}) was not found.", path));
            }
            catch (FormatException)
            {
                throw new FormatException("You have wrong format of credentials in your config! You should fix it.");
            }
        }

        public ulong User
        {
            get { return user_id; }
        }
        
        public ulong Guild
        {
            get { return guild_id; }
        }

        public string Token
        {
            get { return token; }
        }
    }
}

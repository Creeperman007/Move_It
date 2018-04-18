using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using DSharpPlus;
using DSharpPlus.EventArgs;
using System.Reflection;
using System;

namespace Move_It
{
    public class Bot
    {
        // the client instance, this is initialized with the class
        public DiscordClient Client { get; }

        private Assembly assembly = Assembly.GetCallingAssembly();

        // this instantiates the container class and the client
        public Bot(string token)
        {
            // create config from the supplied token
            var cfg = new DiscordConfiguration
            {
                Token = token,
                TokenType = TokenType.Bot,

                AutoReconnect = true,
                LogLevel = LogLevel.Info,
                UseInternalLogHandler = true
            };

            // initialize the client
            this.Client = new DiscordClient(cfg);

            // attach our own debug logger
            this.Client.DebugLogger.LogMessageReceived += this.DebugLogger_LogMessageReceived;
            this.Client.Ready += Client_Ready;
        }

        private Task Client_Ready(ReadyEventArgs e)
        {
            this.Client.DebugLogger.LogMessage(LogLevel.Info, "Move_It", "Client is ready to operate.", DateTime.Now);
            return Task.CompletedTask;
        }

        // this method logs in and starts the client
        public Task StartAsync()
            => this.Client.ConnectAsync();

        // this method logs out and stops the client
        public Task StopAsync()
            => this.Client.DisconnectAsync();

        // this method writes all of bot's log messages to debug output
        private void DebugLogger_LogMessageReceived(object sender, DebugLogMessageEventArgs e)
        { 
            Debug.WriteLine($"[{e.Timestamp.ToString("yyyy-MM-dd HH:mm:ss")}] [{e.Application}] [{e.Level}] {e.Message}");
        }
    }
}

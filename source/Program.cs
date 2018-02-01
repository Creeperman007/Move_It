using System;
using System.IO;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;

namespace Move_It
{
    class Program
    {
        private static string v = "v1.2";
        static void Main(string[] args)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            DiscordClient discord = new DiscordClient(new DiscordConfiguration
            {
                Token = File.ReadAllText("token.txt"),
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Info
            });

            CommandsNextModule commands = discord.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = "/"
            });

            commands.RegisterCommands<Commands>();

            discord.Ready += Client_Ready;
            await discord.ConnectAsync();
            await Task.Delay(-1);
        }

        private static Task Client_Ready(ReadyEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "Move_It", "Client is ready to operate.", DateTime.Now);
            Version.Check(v, e);
            return Task.CompletedTask;
        }
    }
}

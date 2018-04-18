using System;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.Entities;
using System.Collections.Generic;
using System.IO;

namespace Move_It
{
    public partial class Form1 : Form
    {

        // this will hold the thread on which the bot will run
        private Task BotThread { get; set; }

        // this will hold the bot itself
        private Bot Bot { get; set; }

        // this will hold a token required to make the bot quit cleanly
        private CancellationTokenSource TokenSource { get; set; }

        // these are for UI state
        private BotGuild SelectedGuild { get; set; }
        private BotChannel SelectedChannel { get; set; }

        KeyboardHook hook = new KeyboardHook();

        private static TextBox tb_num = new TextBox();
        private static Form InputForm = new Form();
        private static Credentials credentials;

        public Form1()
        {
            Visible = false;
            ShowInTaskbar = false;
            WindowState = FormWindowState.Minimized;
            InitializeComponent();
            hook.KeyPressed +=
            new EventHandler<KeyPressedEventArgs>(hook_KeyPressed);
            // register the control + alt + F12 combination as hot key.
            hook.RegisterHotKey(Move_It.ModifierKeys.Control, Keys.J);
            hook.RegisterHotKey(Move_It.ModifierKeys.Control, Keys.L);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (this.BotThread == null)
            {
                credentials = new Credentials("config.json");
                this.SetProperty(xf => xf.Text, "Move It");
                this.Bot = new Bot(credentials.Token);
                this.TokenSource = new CancellationTokenSource();
                this.BotThread = Task.Run(this.BotThreadCallback);
            }
            Thread.Sleep(400);
            LogMessage(LogLevel.Debug, "Start of log", false);
        }

        private async Task BotThreadCallback()
        {
            await this.Bot.StartAsync().ConfigureAwait(false);
            try
            {
                await Task.Delay(-1, this.TokenSource.Token).ConfigureAwait(false);
            }
            catch { }
            await this.Bot.StopAsync().ConfigureAwait(false);
            this.Bot = null;
            this.TokenSource = null;
            this.BotThread = null;
        }

        public void hook_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            if (e.Key == Keys.J)
            {
                tb_num.Width = 40;
                tb_num.Top = 0;
                tb_num.Left = 0;
                tb_num.Text = "";
                tb_num.Parent = InputForm;
                Button bt_move = new Button();
                bt_move.Height = 20;
                bt_move.Width = 20;
                bt_move.Top = 0;
                bt_move.Left = 50;
                bt_move.Parent = InputForm;
                InputForm.AcceptButton = bt_move;
                InputForm.Width = 50;
                InputForm.Height = 70;
                InputForm.StartPosition = FormStartPosition.Manual;
                InputForm.Left = 0;
                InputForm.Top = Convert.ToInt32(Screen.PrimaryScreen.WorkingArea.Height * 0.60);
                bt_move.Click += new EventHandler(bt_move_Click);
                InputForm.Show();
                InputForm.Activate();
                tb_num.Focus();
            }
            else if (e.Key == Keys.L)
            {
                var guild = Bot.Client.GetGuildAsync(credentials.Guild).Result;
                var user = guild.GetMemberAsync(credentials.User).Result;
                if (user.VoiceState != null)
                {
                    ulong category_id = user.VoiceState.Channel.Parent.Id;
                    List<DiscordChannel> channels = new List<DiscordChannel>();
                    int offset = 0;
                    bool sorting = false;
                    //Adding to List and deletion of channels
                    try
                    {
                        foreach (DiscordChannel chnl in guild.GetChannel(category_id).Children)
                        {
                            if (chnl.Type != ChannelType.Voice)
                                offset++;
                            else
                            {
                                try
                                {
                                    channels.Add(chnl);
                                    LogMessage(LogLevel.Debug, "Added channel to collection - " + chnl.Name);
                                    chnl.DeleteAsync();
                                    LogMessage(LogLevel.Debug, "Deleted channel - " + chnl.Name);
                                }
                                catch (Exception)
                                {
                                    channels.RemoveAt(channels.Count - 1);
                                    sorting = true;
                                    LogMessage(LogLevel.Debug, "Couldn't delete channel, removing from collection - " + chnl.Name);
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        sorting = true;
                    }
                    channels = channels.OrderBy(o => o.Position).ToList();
                    //Creation of channels
                    foreach (DiscordChannel chnl in channels)
                    {
                        guild.CreateChannelAsync(chnl.Name, ChannelType.Voice, chnl.Parent, chnl.Bitrate, chnl.UserLimit, chnl.PermissionOverwrites);
                        LogMessage(LogLevel.Debug, "Created channel - " + chnl.Name);
                    }
                    //Sorting of channels if wrongly ordered
                    if (sorting)
                    {
                        offset += channels[0].Position;
                        channels = channels.OrderBy(o => Convert.ToInt32(Regex.Match(o.Name, @"\d+").Value)).ToList();
                        for (int i = 0; i < channels.Count; i++)
                        {
                            channels[i].ModifyPositionAsync(i + offset);
                            LogMessage(LogLevel.Debug, String.Format("Moved {0} from {1} to {2}", channels[i].Name, channels[i].Position, i + offset));
                            Thread.Sleep(200);
                        }
                    }
                }
                else
                    LogMessage(LogLevel.Warning, "Voice state is null.");
            }
        }

        private async void bt_move_Click(Object sender, EventArgs e)
        {
            try
            {
                SendKeys.Send("%{TAB}");
                InputForm.Hide();
                var user = Bot.Client.GetGuildAsync(credentials.Guild).Result.GetMemberAsync(credentials.User).Result;
                if (user.VoiceState != null)
                {
                    await user.PlaceInAsync(SelectChannel(tb_num.Text, user.VoiceState.Channel.Parent.Id));
                    LogMessage(LogLevel.Debug, "Placed user in selected channel.");
                }
                else
                    LogMessage(LogLevel.Warning, "Voice state is null.");
            }
            catch (NoNullAllowedException)
            {
                LogMessage(LogLevel.Warning, "Selected channel was not found");
            }
            catch (Exception ex)
            {
                LogMessage(LogLevel.Error, ex.ToString());
            }
        }
        
        private DiscordChannel SelectChannel(string text, ulong category)
        {
            var guild = Bot.Client.GetGuildAsync(credentials.Guild).Result;
            foreach (DiscordChannel chnl in guild.Channels)
            {
                if (chnl.Type == ChannelType.Voice && Regex.Match(chnl.Name.ToLower(), @"\d+").Value == text && chnl.Parent.Id == category)
                    return chnl;
            }
            throw new NoNullAllowedException();
        }

        public void LogMessage(LogLevel level, string message, bool append = true)
        {
            DateTime now = DateTime.Now;
            Bot.Client.DebugLogger.LogMessage(level, "Move_It", message, now);
            try
            {
                StreamWriter w = new StreamWriter("log-current_session.txt", append);
                w.WriteLine(String.Format("[{0}] [{1}] {2}", now, level.ToString(), message));
                w.Close();
            }
            catch { }
        }
    }
}

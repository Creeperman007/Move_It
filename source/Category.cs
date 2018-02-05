using System;
using System.IO;
using System.Collections.Generic;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Newtonsoft.Json.Linq;

namespace Move_It
{
    class Category
    {
        public ulong id;
        public string name;

        public Category(string id, string name)
        {
            this.id = ulong.Parse(id);
            this.name = name;
        }
        public static List<Category> Categories()
        {
            List<Category> c = new List<Category>();
            JObject file = JObject.Parse(File.ReadAllText("Categories.json "));
            foreach (JObject obj in file["categories"])
            {
                c.Add(new Category(obj["id"].ToString(), obj["name"].ToString()));
            }
            return c;
        }
        public static DiscordChannel ReturnCategory(CommandContext ctx, int type, List<Category> categories)
        {
            foreach (DiscordChannel chnl in ctx.Guild.Channels)
            {
                if (chnl.Id == categories[type].id)
                    return chnl;
            }
            return null;
        }


        public static void DeleteChannels(CommandContext ctx, int type, List<Category> categories)
        {
            try
            {
                foreach (DiscordChannel chnl in ctx.Guild.Channels)
                {
                    try
                    {
                        if (chnl.ParentId == categories[type].id && chnl.Type == ChannelType.Voice)
                            chnl.DeleteAsync();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
            catch { }
        }
        public static async void CreateChannels(CommandContext ctx, List<DiscordChannel> channels, DiscordChannel category)
        {
            foreach (DiscordChannel chnl in channels)
            {
                try
                {
                    await ctx.Guild.CreateChannelAsync(chnl.Name, ChannelType.Voice, category);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}

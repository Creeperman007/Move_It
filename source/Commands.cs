using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace Move_It
{
    class Commands
    {
        private List<Category> categories = Category.Categories();

        [Command("move")]
        public async Task Move(CommandContext ctx, int type)
        {
            if (ctx.Member.Id == 228248704379912192)
            {
                try
                {
                    await ctx.Guild.CreateChannelAsync("TempMove", ChannelType.Voice);
                    await Task.Delay(300);
                    DiscordChannel chnlTemp = ReturnTemp(ctx);
                    await Task.Delay(300);
                    foreach (DiscordMember usr in ctx.Guild.Members)
                    {
                        try
                        {
                            if (usr.VoiceState.Channel.ParentId == categories[type].id)
                                await usr.PlaceInAsync(chnlTemp);
                        }
                        catch {}
                        await Task.Delay(200);
                    }
                    await Task.Delay(300);
                    await ctx.Guild.GetChannel(chnlTemp.Id).DeleteAsync();
                    await ctx.RespondAsync("Successfully moved users from " + categories[type].name);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    await ctx.Guild.GetChannel(ReturnTemp(ctx).Id).DeleteAsync();
                }
            }
        }

        [Command("list")]
        public async Task List(CommandContext ctx)
        {
            if (ctx.Member.Id == 228248704379912192)
            {
                string list = "````\n";
                int i = 0;
                foreach (Category c in categories)
                {
                    list += String.Format("{0} - {1}\n", i, c.name);
                    i++;
                }
                list += "```";
                await ctx.RespondAsync(list);
            }
        }

        public DiscordChannel ReturnTemp(CommandContext ctx)
        {
            foreach (DiscordChannel chnl in ctx.Guild.Channels)
            {
                if (chnl.Name == "TempMove")
                    return chnl;
            }
            return null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Pathfinder.Services;

namespace Pathfinder.Modules
{
    public class MainModule : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        [Alias("pong")]
        public async Task PingAsync()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var message = await ReplyAsync(".");
            watch.Stop();
            await message.ModifyAsync(msg => msg.Content = string.Format("that took **{0}ms**", watch.ElapsedMilliseconds));
        }

        [Command("adventures")]
        public async Task AdventuresAsync()
        {
            foreach (Adventure adventure in AdventureService.adventures.Values.ToList())
            {
                Embed embed = AdventureService.GetAdventureEmbed(adventure);
                await ReplyAsync(null, embed: embed);
            }
        }

        [Command("startadventure")]
        [Alias("start", "play")]
        public async Task StartAsync([Remainder] string adventurename)
        {
            if (!AdventureService.messageMarkers.ContainsKey(Context.Channel.Id))
            {
                Dictionary<string, AdventureSegment> segments = AdventureService.adventures[adventurename].segments;
                string segIndex = "0";
                IUserMessage msg = await AdventureService.SendSegmentMessage(Context.Channel, adventurename, segIndex);

                messageMarker messageMarker = new messageMarker(msg.Id, adventurename, segIndex);
                AdventureService.messageMarkers.Add(msg.Channel.Id, messageMarker);
            }
            else
            {
                await ReplyAsync("Sorry but only one adventure is allowed in a channel at once");
            }
        }
    }
}

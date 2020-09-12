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
    public class AdventureModule : ModuleBase<SocketCommandContext>
    {
        [Command("adventures"), Summary("Lists all current adventures.")]
        public async Task AdventuresAsync()
        {
            foreach (Adventure adventure in AdventureService.adventures.Values.ToList())
            {
                Embed embed = AdventureService.GetAdventureEmbed(adventure);
                await ReplyAsync(null, embed: embed);
            }
        }

        [Command("startadventure"), Summary("Start an adventure with the desired name.")]
        [Alias("start", "play")]
        public async Task StartAsync([Remainder] string adventurename)
        {
            if (!AdventureService.messageMarkers.ContainsKey(Context.Channel.Id))
            {
                Dictionary<string, AdventureSegment> segments = AdventureService.adventures[adventurename].segments;
                string segIndex = "Start";
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

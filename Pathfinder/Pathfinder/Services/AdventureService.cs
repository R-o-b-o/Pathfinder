using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Pathfinder.Services
{
    public class AdventureService
    {
        public static Dictionary<string, Adventure> adventures = new Dictionary<string, Adventure>();
        private Dictionary<ulong, messageMarker> messageMarkers = new Dictionary<ulong, messageMarker>();

        public AdventureService(DiscordSocketClient client)
        {
            client.ReactionAdded += OnReactionAdded;
            
            LoadAdventureFiles();
        }

        private void LoadAdventureFiles()
        {
            string[] dirs = Directory.GetFiles(Directory.GetCurrentDirectory() + "\\adventures");

            foreach (string dir in dirs)
            {
                Adventure adventure = JsonConvert.DeserializeObject<Adventure>(File.ReadAllText(dir));
                adventures.Add(adventure.config.name, adventure);
            }
        }

        public static Embed GetAdventureEmbed(Adventure adventure)
        {
            AdventureConfig config = adventure.config;

            EmbedBuilder builder = new EmbedBuilder()
                .WithTitle(config.name)
                .WithDescription(config.description)
                .WithColor(new Color(0xCB755A))

                .WithThumbnailUrl(config.imageurl)
                .AddField("plays", config.plays.ToString(), true)
                .AddField("creator", config.creator, true);

            return builder.Build();
        }

        private Task OnReactionAdded(Discord.Cacheable<Discord.IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.User.Value.IsBot)
                return Task.CompletedTask;

            return Task.CompletedTask;
        }
    }
}

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
        public static Dictionary<ulong, messageMarker> messageMarkers = new Dictionary<ulong, messageMarker>();

        private const int charPerSec = 50;

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

            config.creator = config.creator == "" ? "unknown" : config.creator;
            config.description = config.creator == "" ? "this is an adventure" : config.description;

            EmbedBuilder builder = new EmbedBuilder()
                .WithTitle(config.name)
                .WithDescription(config.description)
                .WithColor(new Color(0xCB755A))

                .WithThumbnailUrl(config.imageurl)
                .AddField("plays", config.plays.ToString(), true)
                .AddField("creator", config.creator, true);

            return builder.Build();
        }

        public static Embed GetChoiceEmbed(string adventurename, string segmentname)
        {
            AdventureSegment segment = adventures[adventurename].segments[segmentname];
            var builder = new EmbedBuilder()
                        .WithTitle("choice")
                        .WithDescription(string.Format("``` {0} ```", segment.choicetext))
                        .WithColor(new Color(0xCB755A));
            // if blank default e.g "what do you do?"

            foreach (AdventureChoice choice in segment.choices)
            {
                builder.AddField(choice.text, choice.emote, true);
            }

            return builder.Build();
        }

        public static List<Emoji> GetChoiceEmotes(string adventurename, string segmentname)
        {
            List<Emoji> emotes = adventures[adventurename].segments[segmentname].choices.Select(choice => new Emoji(choice.emote)).ToList();
            return emotes;
        }

        public static async Task<IUserMessage> SendSegmentMessage(ISocketMessageChannel channel, string adventurename, string segIndex)
        {
            AdventureSegment segment = adventures[adventurename].segments[segIndex];
            await channel.SendMessageAsync(string.Format("**{0}**", segment.maintext));

            Embed embed = GetChoiceEmbed(adventurename, segIndex);
            await Task.Delay(Convert.ToInt32(segment.maintext.Length / charPerSec * 1000));
            IUserMessage msg = await channel.SendMessageAsync(null, embed: embed);

            messageMarker messageMarker = new messageMarker(msg.Id, adventurename, segIndex);

            foreach (Emoji emote in GetChoiceEmotes(adventurename, segIndex))
            {
                await msg.AddReactionAsync(emote);
            }
            return msg;
        }

        public static async Task SendEndingMessage(ISocketMessageChannel channel, string adventurename, string segIndex)
        {
            Adventure adventure = adventures[adventurename];
            AdventureSegment segment = adventure.segments[segIndex];
            await channel.SendMessageAsync(string.Format("**{0}**", segment.maintext));

            string ord;
            if (new int[] { 11, 12, 13 }.Contains(adventure.config.plays))
            {
                ord = "th";
            }
            else
            {
                switch (adventure.config.plays.ToString().Last())
                {
                    case '1':
                        ord = "st";
                        break;
                    case '2':
                        ord = "nd";
                        break;
                    case '3':
                        ord = "rd";
                        break;
                    default:
                        ord = "th";
                        break;
                }
            }
            

            string place = adventure.config.plays.ToString() + ord;

            EmbedBuilder builder = new EmbedBuilder()
                        .WithTitle("Ending")
                        .WithDescription(string.Format("``` {0} ```", segment.choicetext))
                        .WithColor(new Color(0xf26500))
                        .WithFooter(footer => footer.WithText(string.Format("You are the {0} player", place)));

            
            Embed embed = builder.Build();
            await Task.Delay(Convert.ToInt32(segment.maintext.Length / charPerSec * 1000));
            await channel.SendMessageAsync(null, embed: embed);
        }

        private void IncrementPlays(string adventurename)
        {
            string dir = Directory.GetCurrentDirectory() + "\\adventures\\" + adventurename + ".json";
            Adventure adventure = adventures[adventurename];

            adventure.config.plays++;
            adventures[adventurename] = adventure;
            string output = JsonConvert.SerializeObject(adventure, Formatting.Indented);
            File.WriteAllText(dir, output);
        }

        private async Task OnReactionAdded(Discord.Cacheable<Discord.IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            string segIndex = "";
            messageMarker messageMarker = messageMarkers[channel.Id];
            if (reaction.User.Value.IsBot || message.Id != messageMarker.messageId)
                return;

            Adventure adventure = adventures[messageMarker.adventureName];
            AdventureSegment segment = adventure.segments[messageMarker.segIndex];

            foreach (AdventureChoice choice in segment.choices)
            {
                Emoji emoji = new Emoji(choice.emote);
                IUserMessage msg = await message.GetOrDownloadAsync();
                if ((await msg.GetReactionUsersAsync(emoji, 5).FlattenAsync()).Count() > 1)
                {
                    segIndex = choice.target;
                }
            }

            if (adventure.segments[segIndex].choices.Count() > 0)
            {
                IUserMessage newmsg = await SendSegmentMessage(channel, messageMarker.adventureName, segIndex);
                messageMarkers[channel.Id] = new messageMarker(newmsg.Id, messageMarker.adventureName, segIndex);
            }
            else
            {
                IncrementPlays(messageMarker.adventureName);
                await SendEndingMessage(channel, messageMarker.adventureName, segIndex);
                messageMarkers.Remove(channel.Id);
            }
        }
    }
}

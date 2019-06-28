using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Discord;
using Discord.Commands;
using Pathfinder.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            string[] dirs = Directory.GetFiles(Directory.GetCurrentDirectory() + "/adventures");
            
            foreach (string dir in dirs)
            {
                dynamic config = (JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(dir))).config;

                var builder = new EmbedBuilder()
                    .WithTitle(config.name.ToString())
                    .WithDescription(config.description.ToString())
                    .WithColor(new Color(0xCB755A))

                    .WithThumbnailUrl(config.imageurl.ToString())
                    .AddField("plays", config.plays.ToString(), true)
                    .AddField("creator", config.creator.ToString(), true);
                var embed = builder.Build();

                await ReplyAsync(null, embed: embed);
            }

            
        }

        [Command("startadventure")]
        [Alias("start", "play")]
        public async Task StartAsync([Remainder] string adventurename)
        {
            JObject json = JObject.Parse(File.ReadAllText(Directory.GetCurrentDirectory() + "/adventures/" + adventurename + ".json"));
            JToken segments = json["segments"];

            string segIndex = "0";
            while (true)
            {
                await ReplyAsync((string)segments[segIndex]["maintext"]);


                var builder = new EmbedBuilder()
                        .WithTitle("choice")
                        .WithDescription(string.Format("```{0}```", (string)segments[segIndex]["choicetext"]))
                        .WithColor(new Color(0xCB755A));

                foreach (JObject choice in segments[segIndex]["choices"])
                {
                    builder.AddField((string)choice["text"], (string)choice["emote"], true);
                }

                var embed = builder.Build();
                await Task.Delay(2000);
                var message = await ReplyAsync(null, embed: embed);

                foreach (JObject choice in segments[segIndex]["choices"])
                {
                    Emoji emoji = new Emoji((string)choice["emote"]);
                    await message.AddReactionAsync(emoji);
                }

                await Task.Delay(5000);

                foreach (JObject choice in segments[segIndex]["choices"])
                {
                    Emoji emoji = new Emoji((string)choice["emote"]);
                    if ((await message.GetReactionUsersAsync(emoji, 5).FlattenAsync()).Count() > 1)
                    {
                        segIndex = (string)choice["target"];
                    }
                }
            }
        }
    }
}

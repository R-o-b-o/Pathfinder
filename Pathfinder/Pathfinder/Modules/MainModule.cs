using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
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

    }
}

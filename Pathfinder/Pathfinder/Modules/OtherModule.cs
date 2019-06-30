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
    public class OtherModule : ModuleBase<SocketCommandContext>
    {
        private CommandService commands;

        public OtherModule(CommandService commands)
        {
            this.commands = commands;
        }

        [Command("ping"), Summary("Find out the bot response time.")]
        [Alias("pong")]
        public async Task PingAsync()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var message = await ReplyAsync(".");
            watch.Stop();
            await message.ModifyAsync(msg => msg.Content = string.Format("that took **{0}ms**", watch.ElapsedMilliseconds));
        }

        [Command("help"), Summary("The help command.")]
        public async Task HelpAsync()
        {
            string helpString = "```";

            IEnumerable<CommandInfo> commandsInfo = commands.Commands;

            foreach (CommandInfo comInfo in commandsInfo)
            {
                string comName = "";
                
                comName = Program.prefix + (comInfo.Module.IsSubmodule ? comInfo.Module.Name + " " : "") + comInfo.Name + " ";

                string comSummary = comInfo.Summary;
                string parameters = "";

                foreach (ParameterInfo comPar in comInfo.Parameters)
                {
                    string paramString = "";
                    if (comPar.IsOptional)
                    {
                        paramString = "[" + comPar.Name + "]";
                    }
                    else
                    {
                        paramString += "<" + comPar.Name + ">";
                    }
                    parameters += paramString + " ";
                }

                helpString += comName + parameters + ": " + comInfo.Summary + "\n";
            }
            await ReplyAsync(helpString + "```");
        }
    }
}

using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Pathfinder.Services;
using DotNetEnv;

namespace Pathfinder
{
    class Program
    {
        public DiscordSocketClient client;

        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            DotNetEnv.Env.Load();

            client = new DiscordSocketClient();
            var services = ConfigureServices();

            services.GetRequiredService<AdventureService>();
            await services.GetRequiredService<CommandHandlingService>().InitializeAsync();

            await client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("token"));
            await client.StartAsync();

            await client.SetGameAsync("p!help");

            Console.WriteLine("bot started");
            await Task.Delay(-1);
        }

        private IServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton(client)
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<HttpClient>()
                .AddSingleton<AdventureService>()
                .BuildServiceProvider();
        }
    }
}

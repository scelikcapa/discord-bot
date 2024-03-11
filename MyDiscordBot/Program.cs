using System;
using System.Threading.Tasks;

using Discord;
using Discord.Net;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;

using MyDiscordBot.Services;
using System.Threading;


public class Program
{
    private DiscordSocketClient _client;

    private readonly IConfiguration _config;
    private InteractionService _commands;
    private ulong _testGuildId;

    public Program()
    {
        // create the configuration
        var _builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile(path: "config.json");  

        // build the configuration and assign to _config          
        _config = _builder.Build();
        _testGuildId = ulong.Parse(_config["TestGuildId"]);
    }

    public static async Task Main(string[] args)
    {
        var program = new Program();
        await program.RunBotAsync();
    }

    public async Task RunBotAsync()
    {
        using (var services = ConfigureServices())
            {
                // get the client and assign to client 
                // you get the services via GetRequiredService<T>
                var client = services.GetRequiredService<DiscordSocketClient>();
                var commands = services.GetRequiredService<InteractionService>();
                _client = client;
                _commands = commands;

                // setup logging and the ready event
                client.Log += LogAsync;
                commands.Log += LogAsync;
                client.Ready += ReadyAsync;
                client.ButtonExecuted += ButtonExecutedAsync;
                //_client.MessageReceived += MessageReceivedAsync;

                // this is where we get the Token value from the configuration file, and start the bot
                // var token = Environment.GetEnvironmentVariable("NameOfYourEnvironmentVariable");
                await client.LoginAsync(TokenType.Bot, _config["Token"]);
                await client.StartAsync();

                // we get the CommandHandler class here and call the InitializeAsync method to start things up for the CommandHandler service
                await services.GetRequiredService<CommandHandler>().InitializeAsync();

                await Task.Delay(Timeout.Infinite);

            }
    }

    public async Task ButtonExecutedAsync(SocketMessageComponent component)
    {
        // We can now check for our custom id
        // Since we set our buttons custom id as 'row_0_button_0', we can check for it like this:
        if(component.Data.CustomId == "row_0_button_0")
        {
            string userNameFirstWord = _client.CurrentUser.Username.Split(' ')[0];
            
            var embed = new EmbedBuilder
                {
                    // Embed property can be set within object initializer
                    Title = userNameFirstWord + " Captcha Authentication",
                    Description = "Please complete this captcha to prove you are a human:"
                };
            // Or with methods
            embed.AddField("Timeout",
                "You have 1 minute to solve this Captcha")
                .WithAuthor(userNameFirstWord + " | Bot", _client.CurrentUser.GetDisplayAvatarUrl())
                .WithColor(Color.DarkPurple);

            
            //Your embed needs to be built before it is able to be sent
            await component.RespondAsync(embed: embed.Build());


            //await component.RespondAsync($"{component.User.Mention} has clicked the button!");
            
        }
    }

    private async Task MessageReceivedAsync(SocketMessage message)
    {
        Console.WriteLine($"{DateTime.Now,-19} [{message.Content,8}] {message.Source}: {message.ToString}");
        if (message.Content.ToLower() != "hello")
        {
            await message.Channel.SendMessageAsync("Hello, Discord!");
        }
    }

    private Task LogAsync(LogMessage message)
    {
        Console.WriteLine(message.ToString());
        return Task.CompletedTask;
    }

    private async Task ReadyAsync()
    {
        if (IsDebug())
        {
            // this is where you put the id of the test discord guild
            System.Console.WriteLine($"In debug mode, adding commands to {_testGuildId}...");
            await _commands.RegisterCommandsToGuildAsync(_testGuildId);
        }
        else
        {
            // this method will add commands globally, but can take around an hour
            await _commands.RegisterCommandsGloballyAsync(true);
        }
        Console.WriteLine($"Connected as -> [{_client.CurrentUser}] :)");
        
        // return Task.CompletedTask;
    }

     // this method handles the ServiceCollection creation/configuration, and builds out the service provider we can call on later
    private ServiceProvider ConfigureServices()
    {
        // this returns a ServiceProvider that is used later to call for those services
        // we can add types we have access to here, hence adding the new using statement:
        // using csharpi.Services;
        return new ServiceCollection()
            .AddSingleton(_config)
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
            .AddSingleton<CommandHandler>()
            .BuildServiceProvider();
    }

    static bool IsDebug ( )
    {
        #if DEBUG
            return true;
        #else
            return false;
        #endif
    }
}
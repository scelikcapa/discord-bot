using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

public class Program
{
    private DiscordSocketClient _client;

    public static async Task Main(string[] args)
    {
        var program = new Program();
        await program.RunBotAsync();
    }

    public async Task RunBotAsync()
    {
        var config = new DiscordSocketConfig()
        {
            GatewayIntents = GatewayIntents.AllUnprivileged
            //MessageCacheSize = 50
        };

        _client = new DiscordSocketClient(config);
        _client.Log += LogAsync;
        _client.Ready += ReadyAsync;
        _client.MessageReceived += MessageReceivedAsync;

        // var token = Environment.GetEnvironmentVariable("NameOfYourEnvironmentVariable");
        await _client.LoginAsync(TokenType.Bot, "MTIxNjMyMjcyODQ5MjI2OTY1OA.GRbmWg._3IRKl5vvn_qExt5xrHlSfwZP38jWevs5Aib-Y");
        await _client.StartAsync();

        await Task.Delay(-1);
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

    private Task ReadyAsync()
    {
        Console.WriteLine($"Connected as -> [{_client.CurrentUser}] :)");
        return Task.CompletedTask;
    }
}
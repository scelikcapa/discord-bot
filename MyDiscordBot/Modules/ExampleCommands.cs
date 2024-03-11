using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace MyDiscordBot.Services
{
    // interation modules must be public and inherit from an IInterationModuleBase
    public class ExampleCommands : InteractionModuleBase<SocketInteractionContext>
    {
        // dependencies can be accessed through Property injection, public properties with public setters will be set by the service provider
        public InteractionService Commands { get; set; }
        private CommandHandler _handler;

        // constructor injection is also a valid way to access the dependecies
        public ExampleCommands (CommandHandler handler)
        {
            _handler = handler;
        }

        /*
        // our first /command!
        [SlashCommand("8ball", "find your answer!")]
        public async Task EightBall(string question)
        {
            // create a list of possible replies
            var replies = new List<string>();

            // add our possible replies
            replies.Add("yes");
            replies.Add("no");
            replies.Add("maybe");
            replies.Add("hazzzzy....");

            // get the answer
            var answer = replies[new Random().Next(replies.Count - 1)];

            // reply with the answer
            await RespondAsync($"You asked: [**{question}**], and your answer is: [**{answer}**]");
        }
        */
      
        [SlashCommand("sendembed", "send an authentication embed to channel!")]
        public async Task SendEmbed()
        {
            
            Emoji emoji;
            var isEmojiExists = Emoji.TryParse(":robot:",out emoji);
            
            string userNameFirstWord = Context.Client.CurrentUser.Username.Split(' ')[0];
            
            var embedBuilder = new EmbedBuilder();
            embedBuilder.WithTitle("Solve Captcha")
                .WithDescription($"Click {emoji.Name} below to request a captcha.\nNote: You must solve a captcha within 10 minutes.")
                .WithAuthor(userNameFirstWord + " | Bot", Context.Client.CurrentUser.GetDisplayAvatarUrl())
                .WithColor(Color.DarkPurple);

            var componentBuilder = new ComponentBuilder()
                            .WithButton(label: "Solve Captcha", "row_0_button_0",ButtonStyle.Secondary,emote: isEmojiExists ? emoji : null);

            // reply with the answer
            await RespondAsync(embed: embedBuilder.Build(),components: componentBuilder.Build());
        }
        
  
    


        /*
        [Command("embed")]
        public async Task SendRichEmbedAsync()
        {
            var embed = new EmbedBuilder
                {
                    // Embed property can be set within object initializer
                    Title = "Hello world!",
                    Description = "I am a description set by initializer."
                };
                // Or with methods
            embed.AddField("Field title",
                "Field value. I also support [hyperlink markdown](https://example.com)!")
                .WithAuthor(Context.Client.CurrentUser)
                .WithFooter(footer => footer.Text = "I am a footer.")
                .WithColor(Color.Blue)
                .WithTitle("I overwrote \"Hello world!\"")
                .WithDescription("I am a description.")
                .WithUrl("https://example.com")
                .WithCurrentTimestamp();

            //Your embed needs to be built before it is able to be sent
            await ReplyAsync(embed: embed.Build());
        }
        */
        
    }
}
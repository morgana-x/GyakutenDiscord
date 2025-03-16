using DSharpPlus.Entities;
using DSharpPlus;
using AttorneyBotV2;
using DSharpPlus.SlashCommands;

namespace AttorneyBot;

// We're sealing it because nothing will be inheriting this class
public sealed class Program
{
    public static async Task Main()
    {
        // For the sake of examples, we're going to load our Discord token from an environment variable.
        string? token = ""; // Environment.GetEnvironmentVariable("DISCORD_TOKEN");
        ulong server_guid = 0;
      //  GameControllerMessage gameController = new();

        if (string.IsNullOrWhiteSpace(token))
        {
            Console.WriteLine("Please specify a token in the DISCORD_TOKEN environment variable.");
            Environment.Exit(1);

            // For the compiler's nullability, unreachable code.
            return;
        }

        // Next, we instantiate our client.
        DiscordConfiguration config = new()
        {
            Token = token,
            Intents = DiscordIntents.AllUnprivileged | DiscordIntents.MessageContents
        };

        DiscordClient client = new(config);
        var slash = client.UseSlashCommands();
        slash.RegisterCommands<SlashCommands>(server_guid);
        client.MessageCreated += ControllerButtons.OnMessageSent;
        client.ComponentInteractionCreated += ControllerButtons.HandleButtonAsync;


        // Now we connect and log in.
        await client.ConnectAsync(new("Ace Attorney", ActivityType.Playing), UserStatus.Online);

        // And now we wait infinitely so that our bot actually stays connected.
        await Task.Delay(-1);
    }
}
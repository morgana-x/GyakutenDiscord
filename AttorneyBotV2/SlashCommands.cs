using DSharpPlus;
using DSharpPlus.SlashCommands;

namespace AttorneyBotV2
{
    public class SlashCommands : ApplicationCommandModule
    {
        [SlashCommand("saiban", "Start a saiban controller")]
        public async Task SaibanCommand(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, GameControllerMessage.GenerateButtonsResponse());
        }
    }
}

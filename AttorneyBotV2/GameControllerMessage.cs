using DSharpPlus.Entities;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace AttorneyBotV2
{
    public enum GBA_CONTROL {
        BTN_A,
        BTN_B,
        BTN_SELECT,
        BTN_START,
        BTN_LEFT,
        BTN_RIGHT,
        BTN_UP,
        BTN_DOWN,
        BTN_RIGHTBUMPER,
        BTN_LEFTBUMPER,
    }

    class GameButton
    {
        public DateTime Expire;
        public GBA_CONTROL Button;
        public Guid ButtonGuid;
        public GameButton(DateTime expire, GBA_CONTROL btn, Guid guid)
        {
            Expire = expire;
            Button = btn;
            ButtonGuid = guid;
        }
    }

    internal class GameControllerMessage
    {
        public static Dictionary<GBA_CONTROL, string> buttonEmojiTranslation = new()
        {
            [GBA_CONTROL.BTN_LEFT] = "⬅️",
            [GBA_CONTROL.BTN_RIGHT] = "➡️",
            [GBA_CONTROL.BTN_DOWN] = "⬇️",
            [GBA_CONTROL.BTN_UP] = "⬆️",
            [GBA_CONTROL.BTN_A] = "🅰️",
            [GBA_CONTROL.BTN_B] = "🅱️",
            [GBA_CONTROL.BTN_SELECT] = "⏸️",
            [GBA_CONTROL.BTN_START] = "▶️",
            [GBA_CONTROL.BTN_RIGHTBUMPER] = "↩️",
            [GBA_CONTROL.BTN_LEFTBUMPER] = "↪️",
        };
        static Dictionary<Guid, GameButton> ActiveButtons = new();

        static async void AddButton(GBA_CONTROL btn, List<DiscordComponent> list) //DiscordMessageBuilder messageBuilder)
        {
            Guid buttonId = Guid.NewGuid();
            ActiveButtons.Add(buttonId, new(DateTime.UtcNow.AddSeconds(10000), btn, buttonId));
            DiscordComponentEmoji emoji = new(buttonEmojiTranslation[btn]);
            DiscordButtonComponent randomButton = new(ButtonStyle.Primary, buttonId.ToString(), "", false, emoji);
            list.Add(randomButton);
        }
        static GBAEmuConnection gBAEmu = new();

        static public async Task OnMessageSent(DiscordClient client, MessageCreateEventArgs eventArgs)
        {
            if (!eventArgs.Message.Content.Equals("!saiban", StringComparison.OrdinalIgnoreCase))
                return;
            await eventArgs.Message.RespondAsync(GenerateButtons());
        }
        static public DiscordMessageBuilder GenerateButtons()
        {
            ActiveButtons.Clear();
            var buttons = Enum.GetValues(typeof(GBA_CONTROL)).Cast<GBA_CONTROL>().ToArray();
            DiscordMessageBuilder messageBuilder = new() { Content = ":video_game:" };

            List<DiscordComponent> firstList = new();
            List<DiscordComponent> secondList = new();
            List<DiscordComponent> thirdList = new();

            for (int i = 0; i < 4; i++)
                AddButton(buttons[i], firstList);
            messageBuilder.AddComponents(firstList);
            for (int i = 4; i < 8; i++)
                AddButton(buttons[i], secondList);
            messageBuilder.AddComponents(secondList);
            for (int i = 9; i >= 8; i--)
                AddButton(buttons[i], thirdList);
            messageBuilder.AddComponents(thirdList);


            return messageBuilder;
        }
        static public DiscordInteractionResponseBuilder GenerateButtonsResponse()
        {
            ActiveButtons.Clear();
            var buttons = Enum.GetValues(typeof(GBA_CONTROL)).Cast<GBA_CONTROL>().ToArray();
            DiscordInteractionResponseBuilder messageBuilder = new() { Content = ":video_game:" };

            List<DiscordComponent> firstList = new();
            List<DiscordComponent> secondList = new();
            List<DiscordComponent> thirdList = new();

            for (int i = 0; i < 4; i++)
                AddButton(buttons[i], firstList);
            messageBuilder.AddComponents(firstList);
            for (int i = 4; i < 8; i++)
                AddButton(buttons[i], secondList);
            messageBuilder.AddComponents(secondList);
            for (int i = 9; i >= 8; i--)
                AddButton(buttons[i], thirdList);
            messageBuilder.AddComponents(thirdList);

            return messageBuilder;
        }
        static public async Task HandleButtonAsync(DiscordClient client, ComponentInteractionCreateEventArgs eventArgs)
        {
            if (!Guid.TryParse(eventArgs.Id, out Guid buttonId))
                return;
            if (!ActiveButtons.ContainsKey(buttonId))
                return;
            //if (DateTime.UtcNow > ActiveButtons[buttonId].Expire)
            //    return;
            gBAEmu.SendKey(((int)ActiveButtons[buttonId].Button).ToString());
            await eventArgs.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, GenerateButtonsResponse());
        }
    }
}

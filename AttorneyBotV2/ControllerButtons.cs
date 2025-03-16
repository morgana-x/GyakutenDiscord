using DSharpPlus.Entities;
using DSharpPlus;
using DSharpPlus.EventArgs;
using AttorneyBotV2.EmuInterface;

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
        BTN_DS_X
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

    internal class ControllerButtons
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
            [GBA_CONTROL.BTN_DS_X] = "❎",
        };
        public static Dictionary<GBA_CONTROL, int> buttonKeyTranslation = new()
        {
            [GBA_CONTROL.BTN_LEFT] = 0x4A, // J
            [GBA_CONTROL.BTN_RIGHT] = 0x4C, // L
            [GBA_CONTROL.BTN_DOWN] = 0x4B, // K
            [GBA_CONTROL.BTN_UP] = 0x49, // I
            [GBA_CONTROL.BTN_A] = 0x5A, // Z
            [GBA_CONTROL.BTN_B] = 0x58, // X
            [GBA_CONTROL.BTN_SELECT] = 0x43, // C
            [GBA_CONTROL.BTN_START] = 0x56, // V
            [GBA_CONTROL.BTN_RIGHTBUMPER] = 0x4E, // N
            [GBA_CONTROL.BTN_LEFTBUMPER] = 0x42, // B
            [GBA_CONTROL.BTN_DS_X] = 0x47 // G
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
        static EmuDSConnection dsEmu = new();

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
            if (dsEmu.Connected && !gBAEmu.Connected)
                AddButton(buttons[10], thirdList);
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
            if (dsEmu.Connected && !gBAEmu.Connected)
                AddButton(buttons[10], thirdList);
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
            if (gBAEmu.Connected)
                gBAEmu.SendKey(((int)ActiveButtons[buttonId].Button).ToString());
            else if (dsEmu.Connected)
                dsEmu.SendKey(buttonKeyTranslation[ActiveButtons[buttonId].Button]);
            await eventArgs.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, GenerateButtonsResponse());
        }
    }
}

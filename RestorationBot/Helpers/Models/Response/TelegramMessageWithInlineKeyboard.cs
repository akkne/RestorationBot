namespace RestorationBot.Helpers.Models.Response;

using global::Telegram.Bot.Types.ReplyMarkups;

public class TelegramMessageWithInlineKeyboard
{
    public string Text { get; set; }
    public InlineKeyboardMarkup KeyboardMarkup { get; set; }

    public static TelegramMessageWithInlineKeyboard Create(string text, InlineKeyboardMarkup keyboardMarkup)
    {
        return new TelegramMessageWithInlineKeyboard
        {
            Text = text,
            KeyboardMarkup = keyboardMarkup
        };
    }
}
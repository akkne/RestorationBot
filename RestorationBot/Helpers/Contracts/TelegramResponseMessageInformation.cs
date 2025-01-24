namespace RestorationBot.Helpers.Contracts;

using Telegram.Bot.Types.ReplyMarkups;

public class TelegramResponseMessageInformation
{
    public string Text { get; set; }
    public InlineKeyboardMarkup KeyboardMarkup { get; set; }

    public static TelegramResponseMessageInformation Create(string text, InlineKeyboardMarkup keyboardMarkup)
    {
        return new TelegramResponseMessageInformation
        {
            Text = text,
            KeyboardMarkup = keyboardMarkup
        };
    }
}
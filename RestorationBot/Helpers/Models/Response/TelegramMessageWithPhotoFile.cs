namespace RestorationBot.Helpers.Models.Response;

using global::Telegram.Bot.Types;

public class TelegramMessageWithPhotoFile
{
    public string Text { get; set; }
    public InputFile Photo { get; set; }

    public static TelegramMessageWithPhotoFile Create(string text, InputFile photo)
    {
        return new TelegramMessageWithPhotoFile { Text = text, Photo = photo };
    }
}
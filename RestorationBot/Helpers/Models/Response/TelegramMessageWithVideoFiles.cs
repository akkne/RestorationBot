namespace RestorationBot.Helpers.Models.Response;

using global::Telegram.Bot.Types;

public class TelegramMessageWithVideoFiles
{
    public string Text { get; set; }
    public List<InputFile> Videos { get; set; }

    public static TelegramMessageWithVideoFiles Create(string text, List<InputFile> videos)
    {
        return new TelegramMessageWithVideoFiles
        {
            Text = text,
            Videos = videos
        };
    }
}
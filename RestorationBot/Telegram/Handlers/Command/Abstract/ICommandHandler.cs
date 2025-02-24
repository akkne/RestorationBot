namespace RestorationBot.Telegram.Handlers.Command.Abstract;

using global::Telegram.Bot;
using global::Telegram.Bot.Types;

public interface ICommandHandler
{
    bool CanHandle(string command);
    Task HandleCommandAsync(string args, Message message, ITelegramBotClient botClient,
                             CancellationToken cancellationToken);
}
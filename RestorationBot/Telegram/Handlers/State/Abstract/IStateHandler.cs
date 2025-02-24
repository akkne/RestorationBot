namespace RestorationBot.Telegram.Handlers.State.Abstract;

using global::Telegram.Bot;
using global::Telegram.Bot.Types;

public interface IStateHandler
{
    bool CanHandle(Message message);

    Task HandleStateAsync(ITelegramBotClient botClient, Message message,
                          CancellationToken cancellationToken);
}
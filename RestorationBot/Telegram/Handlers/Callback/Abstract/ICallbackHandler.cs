namespace RestorationBot.Telegram.Handlers.Callback.Abstract;

using global::Telegram.Bot;
using global::Telegram.Bot.Types;

public interface ICallbackHandler
{
    bool CanHandle(CallbackQuery callbackQuery);

    Task HandleCommandAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery,
                            CancellationToken cancellationToken);
}
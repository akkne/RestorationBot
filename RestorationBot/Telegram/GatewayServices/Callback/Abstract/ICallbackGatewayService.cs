namespace RestorationBot.Telegram.GatewayServices.Callback.Abstract;

using global::Telegram.Bot;
using global::Telegram.Bot.Types;

public interface ICallbackGatewayService
{
    Task HandleCallbackAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery,
                             CancellationToken cancellationToken);
}
namespace RestorationBot.Telegram.GatewayServices.State.Abstract;

using global::Telegram.Bot;
using global::Telegram.Bot.Types;

public interface IStateGatewayService
{
    Task HandleStateAsync(ITelegramBotClient botClient, Message message,
                             CancellationToken cancellationToken);
}
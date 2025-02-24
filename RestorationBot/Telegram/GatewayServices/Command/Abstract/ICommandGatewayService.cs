namespace RestorationBot.Telegram.GatewayServices.Command.Abstract;

using global::Telegram.Bot;
using global::Telegram.Bot.Types;

public interface ICommandGatewayService
{
    Task HandleCommandAsync(string command, string args, Message message, ITelegramBotClient botClient,
                            CancellationToken cancellationToken);
}
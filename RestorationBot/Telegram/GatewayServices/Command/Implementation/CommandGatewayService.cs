namespace RestorationBot.Telegram.GatewayServices.Command.Implementation;

using global::Telegram.Bot;
using global::Telegram.Bot.Types;
using RestorationBot.Telegram.GatewayServices.Command.Abstract;
using RestorationBot.Telegram.Handlers.Command.Abstract;

public class CommandGatewayService : ICommandGatewayService
{
    private readonly IEnumerable<ICommandHandler> _commandHandlers;

    public CommandGatewayService(IEnumerable<ICommandHandler> commandHandlers)
    {
        _commandHandlers = commandHandlers;
    }

    public async Task HandleCommandAsync(string command, string args, Message message, ITelegramBotClient botClient,
                                         CancellationToken cancellationToken)
    {
        ICommandHandler? handler = _commandHandlers.FirstOrDefault(h => h.CanHandle(command));
        if (handler == null)
        {
            throw new InvalidOperationException("Handler not found");
        }
        
        await handler.HandleCommandAsync(command, message, botClient, cancellationToken);
    }
}
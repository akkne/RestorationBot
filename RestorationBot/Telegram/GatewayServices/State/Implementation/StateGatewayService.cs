namespace RestorationBot.Telegram.GatewayServices.State.Implementation;

using Abstract;
using global::Telegram.Bot;
using global::Telegram.Bot.Types;
using Handlers.State.Abstract;

public class StateGatewayService : IStateGatewayService
{
    private readonly IEnumerable<IStateHandler> _stateHandlers;

    public StateGatewayService(IEnumerable<IStateHandler> stateHandlers)
    {
        _stateHandlers = stateHandlers;
    }

    public async Task HandleStateAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        IStateHandler? handler = _stateHandlers.FirstOrDefault(h => h.CanHandle(message));
        if (handler == null)
        {
            throw new InvalidOperationException("Handler not found");
        }
        await handler.HandleStateAsync(botClient, message, cancellationToken);
    }
}
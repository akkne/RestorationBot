namespace RestorationBot.Telegram.GatewayServices.Callback.Implementation;

using Abstract;
using global::Telegram.Bot;
using global::Telegram.Bot.Types;
using Handlers.Callback.Abstract;

public class CallbackGatewayService : ICallbackGatewayService
{
    private readonly IEnumerable<ICallbackHandler> _callbackHandlers;

    public CallbackGatewayService(IEnumerable<ICallbackHandler> callbackHandlers)
    {
        _callbackHandlers = callbackHandlers;
    }

    public async Task HandleCallbackAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery,
                                          CancellationToken cancellationToken)
    {
        ICallbackHandler? handler = _callbackHandlers.FirstOrDefault(h => h.CanHandle(callbackQuery));
        if (handler == null)
        {
            throw new InvalidOperationException("No ICallbackHandler found");
        }
        
        await handler.HandleCommandAsync(botClient, callbackQuery, cancellationToken);
    }
}
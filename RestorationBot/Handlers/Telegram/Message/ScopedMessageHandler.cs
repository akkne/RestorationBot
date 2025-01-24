namespace RestorationBot.Handlers.Telegram.Message;

using global::Telegram.Bot.Types;
using Microsoft.Extensions.Logging;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

public class ScopedMessageHandler : MessageHandler
{
    private readonly ILogger<ScopedMessageHandler> _logger;

    public ScopedMessageHandler(ILogger<ScopedMessageHandler> logger)
    {
        _logger = logger;
    }

    protected override Task HandleAsync(IContainer<Message> container)
    {
        _logger.LogInformation("Received message from {0} with text: {1}", container.Update.Chat.Username,
            container.Update.Text);

        return Task.CompletedTask;
    }
}
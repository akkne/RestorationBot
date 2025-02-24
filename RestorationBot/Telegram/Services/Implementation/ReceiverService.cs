namespace RestorationBot.Telegram.Services.Implementation;

using Abstract;
using global::Telegram.Bot;
using global::Telegram.Bot.Polling;
using global::Telegram.Bot.Types;
using global::Telegram.Bot.Types.Enums;

public class ReceiverService : IReceiverService
{
    private readonly ILogger<ReceiverService> _logger;
    private readonly IUpdateHandler _messageHandler;
    private readonly ITelegramBotClient _telegramBotClient;

    public ReceiverService(ILogger<ReceiverService> logger, IUpdateHandler messageHandler,
                           ITelegramBotClient telegramBotClient)
    {
        _logger = logger;
        _messageHandler = messageHandler;
        _telegramBotClient = telegramBotClient;
    }

    public async Task ReceiveAsync(CancellationToken stoppingToken)
    {
        ReceiverOptions receiverOptions = new()
            { DropPendingUpdates = true, AllowedUpdates = [UpdateType.Message, UpdateType.CallbackQuery] };

        User bot = await _telegramBotClient.GetMe(stoppingToken);
        _logger.LogInformation("Start receiving updates for {BotName}", bot.Username);

        await _telegramBotClient.ReceiveAsync(_messageHandler, receiverOptions, stoppingToken);
    }
}
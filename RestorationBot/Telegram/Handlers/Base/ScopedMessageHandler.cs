namespace RestorationBot.Telegram.Handlers.Base;

using FinalStateMachine.StateStorage.StorageCleaner.Abstract;
using GatewayServices.Callback.Abstract;
using GatewayServices.Command.Abstract;
using GatewayServices.State.Abstract;
using global::Telegram.Bot;
using global::Telegram.Bot.Exceptions;
using global::Telegram.Bot.Polling;
using global::Telegram.Bot.Types;
using global::Telegram.Bot.Types.Enums;

public class ScopedMessageHandler : IUpdateHandler
{
    private const string ErrorMessage = """
                                        Что-то пошло не так, попробуйте позже.
                                        Если ошибка не пройдет, пожалуйста обратитесь в тех поддержку.
                                        """;

    private readonly ICallbackGatewayService _callbackGatewayService;
    private readonly ICommandGatewayService _commandGatewayService;

    private readonly ILogger<ScopedMessageHandler> _logger;
    private readonly IStateGatewayService _stateGatewayService;
    private readonly IStateStorageCleanerService _stateStorageCleanerService;

    public ScopedMessageHandler(ILogger<ScopedMessageHandler> logger, ICommandGatewayService commandGatewayService,
                                ICallbackGatewayService callbackGatewayService,
                                IStateGatewayService stateGatewayService,
                                IStateStorageCleanerService stateStorageCleanerService)
    {
        _logger = logger;
        _commandGatewayService = commandGatewayService;
        _callbackGatewayService = callbackGatewayService;
        _stateGatewayService = stateGatewayService;
        _stateStorageCleanerService = stateStorageCleanerService;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
                                        CancellationToken cancellationToken)
    {
        if (update.Message?.Type == MessageType.Text)
        {
            Message message = update.Message;
            await HandleTextMessageAsync(botClient, message, cancellationToken);
            return;
        }

        if (update.CallbackQuery != null)
        {
            CallbackQuery callbackQuery = update.CallbackQuery;
            await HandleCallbackAsync(botClient, callbackQuery, cancellationToken);
        }
    }

    public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source,
                                       CancellationToken cancellationToken)
    {
        _logger.LogInformation("HandleError: {Exception}", exception);

        if (exception is RequestException) await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
    }

    private async Task HandleCallbackAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery,
                                           CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Received callback: {text} from user with username: {username} with id: {id}",
            callbackQuery.Data, callbackQuery.From.Username, callbackQuery.From.Id);

        try
        {
            await botClient.AnswerCallbackQuery(callbackQuery.Id, cancellationToken: cancellationToken);
            await _callbackGatewayService.HandleCallbackAsync(botClient, callbackQuery, cancellationToken);
        }
        catch (InvalidOperationException exception)
        {
            _logger.LogError(exception.Message);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception.Message);
            await botClient.SendMessage(callbackQuery.From.Id, ErrorMessage, cancellationToken: cancellationToken);
        }
    }

    private async Task HandleTextMessageAsync(ITelegramBotClient botClient, Message message,
                                              CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Received message: {text} from user with username: {username} with id: {id}",
            message.Text, message.From!.Username, message.From.Id);

        if (message.Text!.StartsWith('/'))
        {
            await OnCommandAsync(botClient, message, cancellationToken);
            return;
        }

        await OnStateAsync(botClient, message, cancellationToken);
    }

    private async Task OnStateAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        try
        {
            await _stateGatewayService.HandleStateAsync(botClient, message, cancellationToken);
        }
        catch (InvalidOperationException exception)
        {
            _logger.LogError(exception.Message);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception.Message);
            await botClient.SendMessage(message.From!.Id, ErrorMessage, cancellationToken: cancellationToken);
        }
    }

    private async Task OnCommandAsync(ITelegramBotClient botClient, Message message,
                                      CancellationToken cancellationToken)
    {
        _stateStorageCleanerService.RemoveAllStates();

        (string? command, string? args) = GetCommandArgumentsObjectAsync(message.Text!);

        try
        {
            await _commandGatewayService.HandleCommandAsync(command, args, message, botClient, cancellationToken);
        }
        catch (InvalidOperationException exception)
        {
            _logger.LogError(exception.Message);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception.Message);
            await botClient.SendMessage(message.From!.Id, ErrorMessage, cancellationToken: cancellationToken);
        }
    }

    private static (string, string) GetCommandArgumentsObjectAsync(string messageText)
    {
        int spaceIndex = messageText.IndexOf(' ');
        if (spaceIndex < 0) spaceIndex = messageText.Length;

        string command = messageText[..spaceIndex].ToLower();
        string args = messageText[spaceIndex..].TrimStart();

        return (command, args);
    }
}
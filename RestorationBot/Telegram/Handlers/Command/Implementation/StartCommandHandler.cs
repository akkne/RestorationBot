namespace RestorationBot.Telegram.Handlers.Command.Implementation;

using Abstract;
using FinalStateMachine.OperationsConfiguration.OperationStatesProfiles.UserRegistration;
using FinalStateMachine.States;
using FinalStateMachine.States.Implementation;
using FinalStateMachine.StateStorage.Particular.Abstract;
using FinalStateMachine.StateStorage.Particular.Abstract.Base;
using FinalStateMachine.StateStorage.Particular.Abstract.Certain;
using global::Telegram.Bot;
using global::Telegram.Bot.Types;
using global::Telegram.Bot.Types.Enums;
using global::Telegram.Bot.Types.ReplyMarkups;
using Helpers.Abstract;
using Helpers.Contracts;
using RestorationBot.Services.Abstract;
using Shared.Enums;
using BusinessUser = Models.User;

public class StartCommandHandler : ICommandHandler
{
    private const string BaseCommandName = "/start";

    private static readonly string[] Commands = ["/start", "/training", "/reports", "/change"];

    private readonly IUserRegistrationStateStorageService _storageService;
    private readonly ICallbackGenerator _callbackGenerator;
    private readonly IUserRegistrationService _userRegistrationService;

    public StartCommandHandler(IUserRegistrationStateStorageService storageService, IUserRegistrationService userRegistrationService, ICallbackGenerator callbackGenerator)
    {
        _storageService = storageService;
        _userRegistrationService = userRegistrationService;
        _callbackGenerator = callbackGenerator;
    }

    public bool CanHandle(string command)
    {
        return command == BaseCommandName;
    }

    public async Task HandleCommandAsync(string args, Message message, ITelegramBotClient botClient,
                                         CancellationToken cancellationToken)
    {
        ReplyMarkup replyMarkup = new ReplyKeyboardMarkup(Commands
           .Select(x => new KeyboardButton(x)));
        
        UserRegistrationState state = _storageService.GetOrAddState(message.From!.Id);
        if (state.StateMachine.State != UserRegistrationStateProfile.Ready)
        {
            _storageService.TryRemove(message.From!.Id);
            _storageService.GetOrAddState(message.From!.Id);
        }
        
        await state.StateMachine.FireAsync(UserRegistrationTriggerProfile.Begin, cancellationToken);
        
        BusinessUser? user = await _userRegistrationService.GetByTelegramIdAsync(message.From.Id, cancellationToken);
        if (user != null)
        {
            await botClient.SendMessage(message.From.Id, "Здравствуйте! Я ваш виртуальный помощник для реабилитации после эндопротезирования коленного сустава.", replyMarkup: replyMarkup, cancellationToken: cancellationToken);
            return;
        }
        
        const string welcomeMessage = """
                                      Здравствуйте! Я ваш виртуальный помощник для реабилитации после эндопротезирования коленного сустава.

                                      С моей помощью вы сможете:
                                      - Узнать безопасные упражнения для восстановления.
                                      - Оценить своё состояние.
                                      - Получить рекомендации, если вы испытываете трудности.
                                      """;
        
        await botClient.SendMessage(message.From.Id, welcomeMessage, replyMarkup: replyMarkup, cancellationToken: cancellationToken);
        
        const string choosingStepMessage = """
                                           Пожалуйста, выберите этап вашей реабилитации:
                                           1️⃣ Ранний этап (0–2 недели после операции).

                                           2️⃣ Средний этап (2–6 недель после операции).

                                           3️⃣ Поздний этап (6 недель и более после операции).
                                           """;

        List<InlineKeyboardButton> inlineKeyboardButtons =
            new List<int> { 1, 2, 3 }.Select(x =>
                new InlineKeyboardButton(x.ToString())
                {
                    CallbackData = _callbackGenerator.GenerateCallbackOnChoosingRestorationStep((RestorationSteps) (x - 1))
                }).ToList();

        await botClient.SendMessage(message.From.Id, choosingStepMessage, ParseMode.Html, replyMarkup: new InlineKeyboardMarkup(inlineKeyboardButtons), cancellationToken: cancellationToken);
    }
}
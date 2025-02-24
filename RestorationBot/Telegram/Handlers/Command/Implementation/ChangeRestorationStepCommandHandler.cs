namespace RestorationBot.Telegram.Handlers.Command.Implementation;

using Abstract;
using FinalStateMachine.OperationsConfiguration.OperationStatesProfiles.UserChangeRestoratonStep;
using FinalStateMachine.States.Implementation;
using FinalStateMachine.StateStorage.Particular.Abstract.Certain;
using global::Telegram.Bot;
using global::Telegram.Bot.Types;
using global::Telegram.Bot.Types.Enums;
using global::Telegram.Bot.Types.ReplyMarkups;
using Helpers.Abstract;
using Shared.Enums;

public class ChangeRestorationStepCommandHandler : ICommandHandler
{
    private const string BaseCommandName = "/change";

    private readonly ICallbackGenerator _callbackGenerator;
    private readonly IUserChangeRestorationStepStateStorageService _storageService;

    public ChangeRestorationStepCommandHandler(ICallbackGenerator callbackGenerator,
                                               IUserChangeRestorationStepStateStorageService storageService)
    {
        _callbackGenerator = callbackGenerator;
        _storageService = storageService;
    }

    public bool CanHandle(string command)
    {
        return command == BaseCommandName;
    }

    public async Task HandleCommandAsync(string args, Message message, ITelegramBotClient botClient,
                                         CancellationToken cancellationToken)
    {
        UserChangeRestorationStepState state = _storageService.GetOrAddState(message.From!.Id);
        if (state.StateMachine.State != UserChangeRestorationStepStateProfile.Ready)
        {
            _storageService.TryRemove(message.From!.Id);
            _storageService.GetOrAddState(message.From!.Id);
        }

        await state.StateMachine.FireAsync(UserChangeRestorationStepTriggerProfile.Begin, cancellationToken);

        const string changingStepMessage = """
                                           Для того, чтобы изменить этап реабилитации, выберите из этапов ниже:

                                           1️⃣ Ранний этап (0–2 недели после операции).

                                           2️⃣ Средний этап (2–6 недель после операции).

                                           3️⃣ Поздний этап (6 недель и более после операции).
                                           """;

        List<InlineKeyboardButton> inlineKeyboardButtons =
            new List<int> { 1, 2, 3 }.Select(x =>
                new InlineKeyboardButton(x.ToString())
                {
                    CallbackData =
                        _callbackGenerator.GenerateCallbackOnChangingRestorationStep((RestorationSteps)(x - 1))
                }).ToList();

        await botClient.SendMessage(message.From.Id, changingStepMessage, ParseMode.Html,
            replyMarkup: new InlineKeyboardMarkup(inlineKeyboardButtons), cancellationToken: cancellationToken);
    }
}
namespace RestorationBot.Telegram.Handlers.State.Implementation.UserRegistration;

using Abstract;
using FinalStateMachine.OperationsConfiguration.OperationStatesProfiles.UserRegistration;
using FinalStateMachine.States.Implementation;
using FinalStateMachine.StateStorage.Particular.Abstract.Certain;
using global::Telegram.Bot;
using global::Telegram.Bot.Types;
using global::Telegram.Bot.Types.ReplyMarkups;
using Helpers.Abstract;
using Shared.Enums;

public class AgeEnteringStateHandler : IStateHandler
{
    private readonly ICallbackGenerator _callbackGenerator;
    private readonly IUserRegistrationStateStorageService _particularStateStorageService;

    public AgeEnteringStateHandler(IUserRegistrationStateStorageService particularStateStorageService,
                                   ICallbackGenerator callbackGenerator)
    {
        _particularStateStorageService = particularStateStorageService;
        _callbackGenerator = callbackGenerator;
    }

    public bool CanHandle(Message message)
    {
        UserRegistrationState state = _particularStateStorageService.GetOrAddState(message.From!.Id);
        return state.StateMachine.State == UserRegistrationStateProfile.AgeEntering;
    }

    public async Task HandleStateAsync(ITelegramBotClient botClient, Message message,
                                       CancellationToken cancellationToken)
    {
        UserRegistrationState state = _particularStateStorageService.GetOrAddState(message.From!.Id);

        if (!int.TryParse(message.Text, out int age)) throw new ArgumentException("Invalid age");
        state.Age = age;

        await state.StateMachine.FireAsync(UserRegistrationTriggerProfile.AgeEntered, cancellationToken);

        string responseOnGettingSex = """
                                      Хорошо, теперь укажите ваш пола
                                      """;

        List<InlineKeyboardButton> inlineKeyboardButtons =
        [
            new("Мужской")
            {
                CallbackData = _callbackGenerator.GenerateCallbackOnChoosingSex(Sex.Male)
            },
            new("Женский")
            {
                CallbackData = _callbackGenerator.GenerateCallbackOnChoosingSex(Sex.Female)
            }
        ];

        await botClient.SendMessage(message.From.Id, responseOnGettingSex,
            replyMarkup: new InlineKeyboardMarkup(inlineKeyboardButtons), cancellationToken: cancellationToken);
    }
}
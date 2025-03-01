namespace RestorationBot.Telegram.Handlers.State.Implementation.UserTraining.BloodPressure;

using Abstract;
using FinalStateMachine.OperationsConfiguration.OperationStatesProfiles.UserTraining;
using FinalStateMachine.States.Implementation;
using FinalStateMachine.StateStorage.Particular.Abstract.Certain;
using global::Telegram.Bot;
using global::Telegram.Bot.Types;
using global::Telegram.Bot.Types.ReplyMarkups;
using Helpers.Abstract;
using Helpers.Abstract.MessageGenerators;
using Helpers.Models.Response;
using RestorationBot.Services.Abstract;

public class PreBloodPressureEnteringStateHandler : IStateHandler
{
    private readonly ICallbackGenerator _callbackGenerator;
    private readonly IUserTrainingStateStorageService _userTrainingStateStorageService;

    public PreBloodPressureEnteringStateHandler(IUserTrainingStateStorageService userTrainingStateStorageService,
                                                IUserTrainingService userTrainingService,
                                                ICallbackGenerator callbackGenerator,
                                                IRestorationStepMessageGenerator restorationStepMessageGenerator,
                                                IUserRegistrationService userRegistrationService)
    {
        _userTrainingStateStorageService = userTrainingStateStorageService;
        _callbackGenerator = callbackGenerator;
    }

    public bool CanHandle(Message message)
    {
        UserTrainingState state = _userTrainingStateStorageService.GetOrAddState(message.From!.Id);
        return state.StateMachine.State == UserTrainingStateProfile.PreBloodPressureEntering;
    }

    public async Task HandleStateAsync(ITelegramBotClient botClient, Message message,
                                       CancellationToken cancellationToken)
    {
        UserTrainingState state = _userTrainingStateStorageService.GetOrAddState(message.From!.Id);

        if (!double.TryParse(message.Text, out double bloodPressure))
            throw new ArgumentException("Invalid blood pressure provided");
        state.PreBloodPressure = bloodPressure;

        await state.StateMachine.FireAsync(UserTrainingTriggerProfile.PreBloodPressureEntered, cancellationToken);

        TelegramMessageWithInlineKeyboard response = GenerateMessage();

        await botClient.SendMessage(message.From.Id, response.Text, replyMarkup: response.KeyboardMarkup,
            cancellationToken: cancellationToken);
    }

    private TelegramMessageWithInlineKeyboard GenerateMessage()
    {
        const string text = """
                            Выберите тип упражнений, которые хотите выполнить:

                            1️⃣ Идеомоторные упражнения.
                            2️⃣ Физические упражнения.
                            """;

        List<int> variety = Enumerable.Range(0, 2).ToList();

        List<InlineKeyboardButton> inlineKeyboardButtons =
            variety.Select(x =>
                new InlineKeyboardButton((x + 1).ToString())
                {
                    CallbackData =
                        _callbackGenerator.GenerateCallbackOnChoosingExercisePoint(
                            x)
                }).ToList();

        return TelegramMessageWithInlineKeyboard.Create(text, new InlineKeyboardMarkup(inlineKeyboardButtons));
    }
}
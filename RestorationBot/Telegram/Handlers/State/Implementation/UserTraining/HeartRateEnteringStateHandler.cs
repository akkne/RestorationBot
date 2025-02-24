namespace RestorationBot.Telegram.Handlers.State.Implementation.UserTraining;

using Abstract;
using FinalStateMachine.OperationsConfiguration.OperationStatesProfiles.UserTraining;
using FinalStateMachine.States.Implementation;
using FinalStateMachine.StateStorage.Particular.Abstract.Certain;
using global::Telegram.Bot;
using global::Telegram.Bot.Types;
using global::Telegram.Bot.Types.Enums;

public class HeartRateEnteringStateHandler : IStateHandler
{
    private readonly IUserTrainingStateStorageService _userTrainingStateStorageService;

    public HeartRateEnteringStateHandler(IUserTrainingStateStorageService userTrainingStateStorageService)
    {
        _userTrainingStateStorageService = userTrainingStateStorageService;
    }

    public bool CanHandle(Message message)
    {
        UserTrainingState state = _userTrainingStateStorageService.GetOrAddState(message.From!.Id);
        return state.StateMachine.State == UserTrainingStateProfile.HeartRateEntering;
    }

    public async Task HandleStateAsync(ITelegramBotClient botClient, Message message,
                                       CancellationToken cancellationToken)
    {
        UserTrainingState state = _userTrainingStateStorageService.GetOrAddState(message.From!.Id);

        if (!double.TryParse(message.Text, out double heartRate))
            throw new ArgumentException("Invalid heart rate provided");
        state.HeartRate = heartRate;

        const string messageOnGettingBloodPressure = """
                                                     Каким был ваш показатель артериального давления после выполнения тренировки?
                                                     """;
        await state.StateMachine.FireAsync(UserTrainingTriggerProfile.HeartRateEntered, cancellationToken);
        await botClient.SendMessage(message.From!.Id, messageOnGettingBloodPressure, ParseMode.Html,
            cancellationToken: cancellationToken);
    }
}
namespace RestorationBot.Telegram.Handlers.State.Implementation.UserTraining.HeartRate;

using Abstract;
using FinalStateMachine.OperationsConfiguration.OperationStatesProfiles.UserTraining;
using FinalStateMachine.States.Implementation;
using FinalStateMachine.StateStorage.Particular.Abstract.Certain;
using global::Telegram.Bot;
using global::Telegram.Bot.Types;
using global::Telegram.Bot.Types.Enums;

public class PostHeartRateEnteringStateHandler : IStateHandler
{
    private readonly IUserTrainingStateStorageService _userTrainingStateStorageService;

    public PostHeartRateEnteringStateHandler(IUserTrainingStateStorageService userTrainingStateStorageService)
    {
        _userTrainingStateStorageService = userTrainingStateStorageService;
    }

    public bool CanHandle(Message message)
    {
        UserTrainingState state = _userTrainingStateStorageService.GetOrAddState(message.From!.Id);
        return state.StateMachine.State == UserTrainingStateProfile.PostHeartRateEntering;
    }

    public async Task HandleStateAsync(ITelegramBotClient botClient, Message message,
                                       CancellationToken cancellationToken)
    {
        UserTrainingState state = _userTrainingStateStorageService.GetOrAddState(message.From!.Id);

        if (!double.TryParse(message.Text, out double heartRate))
            throw new ArgumentException("Invalid heart rate provided");
        state.PostHeartRate = heartRate;

        const string messageOnGettingBloodPressure = """
                                                     Каким был показатель вашего артериального давления после выполнения тренировки?
                                                     """;
        await state.StateMachine.FireAsync(UserTrainingTriggerProfile.PostHeartRateEntered, cancellationToken);
        await botClient.SendMessage(message.From!.Id, messageOnGettingBloodPressure, ParseMode.Html,
            cancellationToken: cancellationToken);
    }
}
namespace RestorationBot.Telegram.Handlers.State.Implementation.UserTraining.HeartRate;

using Abstract;
using FinalStateMachine.OperationsConfiguration.OperationStatesProfiles.UserTraining;
using FinalStateMachine.States.Implementation;
using FinalStateMachine.StateStorage.Particular.Abstract.Certain;
using global::Telegram.Bot;
using global::Telegram.Bot.Types;
using global::Telegram.Bot.Types.Enums;

public class PreHeartRateEnteringStateHandler : IStateHandler
{
    private readonly IUserTrainingStateStorageService _userTrainingStateStorageService;

    public PreHeartRateEnteringStateHandler(IUserTrainingStateStorageService userTrainingStateStorageService)
    {
        _userTrainingStateStorageService = userTrainingStateStorageService;
    }

    public bool CanHandle(Message message)
    {
        UserTrainingState state = _userTrainingStateStorageService.GetOrAddState(message.From!.Id);
        return state.StateMachine.State == UserTrainingStateProfile.PreHeartRateEntering;
    }

    public async Task HandleStateAsync(ITelegramBotClient botClient, Message message,
                                       CancellationToken cancellationToken)
    {
        UserTrainingState state = _userTrainingStateStorageService.GetOrAddState(message.From!.Id);

        if (!double.TryParse(message.Text, out double heartRate))
            throw new ArgumentException("Invalid heart rate provided");
        state.PreHeartRate = heartRate;

        const string messageOnGettingBloodPressure = """
                                                     Какой у вас показатель артеривально давления? (перед выполнением упражнений)
                                                     """;
        await state.StateMachine.FireAsync(UserTrainingTriggerProfile.PreHeartRateEntered, cancellationToken);
        await botClient.SendMessage(message.From!.Id, messageOnGettingBloodPressure, ParseMode.Html,
            cancellationToken: cancellationToken);
    }
}
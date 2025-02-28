namespace RestorationBot.Telegram.Handlers.State.Implementation.UserTraining.BloodPressure;

using Abstract;
using FinalStateMachine.OperationsConfiguration.OperationStatesProfiles.UserTraining;
using FinalStateMachine.States.Implementation;
using FinalStateMachine.StateStorage.Particular.Abstract.Certain;
using global::Telegram.Bot;
using global::Telegram.Bot.Types;
using Helpers.Abstract;
using Helpers.Abstract.MessageGenerators;
using Helpers.Models.Response;
using RestorationBot.Services.Abstract;
using User = Models.User;

public class PreBloodPressureEnteringStateHandler : IStateHandler
{
    private readonly IRestorationStepMessageGenerator _restorationStepMessageGenerator;
    private readonly IUserRegistrationService _userRegistrationService;
    private readonly IUserTrainingStateStorageService _userTrainingStateStorageService;

    public PreBloodPressureEnteringStateHandler(IUserTrainingStateStorageService userTrainingStateStorageService,
                                                IUserTrainingService userTrainingService,
                                                ICallbackGenerator callbackGenerator,
                                                IRestorationStepMessageGenerator restorationStepMessageGenerator,
                                                IUserRegistrationService userRegistrationService)
    {
        _userTrainingStateStorageService = userTrainingStateStorageService;
        _restorationStepMessageGenerator = restorationStepMessageGenerator;
        _userRegistrationService = userRegistrationService;
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

        User user = await _userRegistrationService.GetByTelegramIdAsync(state.UserId, cancellationToken)
                 ?? throw new NullReferenceException("User not found");

        TelegramMessageWithInlineKeyboard response =
            _restorationStepMessageGenerator.GetRestorationStepMessage(user.RestorationStep);

        await botClient.SendMessage(message.From.Id, response.Text, replyMarkup: response.KeyboardMarkup,
            cancellationToken: cancellationToken);
    }
}
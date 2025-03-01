namespace RestorationBot.Telegram.Handlers.Callback.Implementation.UserTraining;

using System.Text.RegularExpressions;
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
using BusinessUser = Models.User;

public class ExercisePointEnteringCallbackHandler : ICallbackHandler
{
    private readonly ICallbackGenerator _callbackGenerator;
    private readonly IRestorationStepMessageGenerator _restorationStepMessageGenerator;
    private readonly IUserTrainingStateStorageService _storageService;
    private readonly IUserRegistrationService _userRegistrationService;

    public ExercisePointEnteringCallbackHandler(ICallbackGenerator callbackGenerator,
                                                IUserTrainingStateStorageService storageService,
                                                IUserRegistrationService userRegistrationService,
                                                IRestorationStepMessageGenerator restorationStepMessageGenerator)
    {
        _callbackGenerator = callbackGenerator;
        _storageService = storageService;
        _userRegistrationService = userRegistrationService;
        _restorationStepMessageGenerator = restorationStepMessageGenerator;
    }

    public bool CanHandle(CallbackQuery callbackQuery)
    {
        Match match = _callbackGenerator.GetCallbackRegexOnChoosingExercisePoint().Match(callbackQuery.Data!);
        UserTrainingState state = _storageService.GetOrAddState(callbackQuery.From.Id);

        return match.Success &&
               state.StateMachine.State == UserTrainingStateProfile.ExercisePointChoosing;
    }

    public async Task HandleCommandAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery,
                                         CancellationToken cancellationToken)
    {
        Match match = _callbackGenerator.GetCallbackRegexOnChoosingExercisePoint().Match(callbackQuery.Data!);
        if (!match.Success) throw new ArgumentException("Invalid callback query");

        int index = int.Parse(match.Groups["index"].Value);

        await OnChoosingExercisePointAsync(callbackQuery.From, index, botClient, cancellationToken);
    }

    private async Task OnChoosingExercisePointAsync(User userFrom, int exercisePoint, ITelegramBotClient botClient,
                                                    CancellationToken cancellationToken = default)
    {
        UserTrainingState state = _storageService.GetOrAddState(userFrom.Id);

        switch (exercisePoint)
        {
            case 0:
                await OnIdeomotorExercisePointAsync(userFrom, botClient, cancellationToken);
                state.ExercisePointChosen = 0;
                break;
            case 1:
                await OnPhysicalExercisePointAsync(userFrom, botClient, cancellationToken);
                state.ExercisePointChosen = 1;
                break;
            default: throw new ArgumentException("Invalid exercise point");
        }

        await state.StateMachine.FireAsync(UserTrainingTriggerProfile.ExercisePointChosen);
    }

    private async Task OnIdeomotorExercisePointAsync(User userFrom, ITelegramBotClient botClient,
                                                     CancellationToken cancellationToken = default)
    {
        BusinessUser user = await _userRegistrationService.GetByTelegramIdAsync(userFrom.Id, cancellationToken)
                         ?? throw new NullReferenceException("User not found");

        TelegramMessageWithInlineKeyboard response =
            _restorationStepMessageGenerator.GetIdeomotorTrainingMessage(user.RestorationStep);

        await botClient.SendMessage(userFrom.Id, response.Text, replyMarkup: response.KeyboardMarkup,
            cancellationToken: cancellationToken);
    }

    private async Task OnPhysicalExercisePointAsync(User userFrom, ITelegramBotClient botClient,
                                                    CancellationToken cancellationToken = default)
    {
        BusinessUser user = await _userRegistrationService.GetByTelegramIdAsync(userFrom.Id, cancellationToken)
                         ?? throw new NullReferenceException("User not found");

        TelegramMessageWithInlineKeyboard response =
            _restorationStepMessageGenerator.GetPhysicalTrainingMessage(user.RestorationStep);

        await botClient.SendMessage(userFrom.Id, response.Text, replyMarkup: response.KeyboardMarkup,
            cancellationToken: cancellationToken);
    }
}
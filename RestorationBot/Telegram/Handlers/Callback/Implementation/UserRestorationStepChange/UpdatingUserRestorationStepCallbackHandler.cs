namespace RestorationBot.Telegram.Handlers.Callback.Implementation.UserRestorationStepChange;

using System.Text.RegularExpressions;
using Abstract;
using FinalStateMachine.OperationsConfiguration.OperationStatesProfiles.UserChangeRestoratonStep;
using FinalStateMachine.States.Implementation;
using FinalStateMachine.StateStorage.Particular.Abstract.Certain;
using global::Telegram.Bot;
using global::Telegram.Bot.Types;
using global::Telegram.Bot.Types.Enums;
using Helpers.Abstract;
using RestorationBot.Services.Abstract;
using Shared.Enums;

public class UpdatingUserRestorationStepCallbackHandler : ICallbackHandler
{
    private readonly ICallbackGenerator _callbackGenerator;
    private readonly IUserChangeRestorationStepStateStorageService _storageService;
    private readonly IUserRegistrationService _userRegistrationService;

    public UpdatingUserRestorationStepCallbackHandler(IUserChangeRestorationStepStateStorageService storageService,
                                                      ICallbackGenerator callbackGenerator,
                                                      IUserRegistrationService userRegistrationService)
    {
        _storageService = storageService;
        _callbackGenerator = callbackGenerator;
        _userRegistrationService = userRegistrationService;
    }

    public bool CanHandle(CallbackQuery callbackQuery)
    {
        Match match = _callbackGenerator.GetCallbackRegexOnChangingRestorationStep().Match(callbackQuery.Data!);
        UserChangeRestorationStepState state = _storageService.GetOrAddState(callbackQuery.From.Id);

        return match.Success &&
               state.StateMachine.State == UserChangeRestorationStepStateProfile.RestorationStepUpdating;
    }

    public async Task HandleCommandAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery,
                                         CancellationToken cancellationToken)
    {
        Match match = _callbackGenerator.GetCallbackRegexOnChangingRestorationStep().Match(callbackQuery.Data!);
        if (!match.Success) throw new ArgumentException("Invalid callback query");

        RestorationSteps restorationStep =
            (RestorationSteps)int.Parse(match.Groups["index"].Value);

        await OnUpdatingExerciseAsync(restorationStep, botClient, callbackQuery.From, cancellationToken);
    }

    private async Task OnUpdatingExerciseAsync(RestorationSteps restorationStep, ITelegramBotClient botClient,
                                               User userFrom, CancellationToken cancellationToken)
    {
        UserChangeRestorationStepState state = _storageService.GetOrAddState(userFrom.Id);
        state.RestorationStep = restorationStep;

        await state.StateMachine.FireAsync(UserChangeRestorationStepTriggerProfile.RestorationStepUpdated,
            cancellationToken);

        await _userRegistrationService.UpdateUserRestorationStepAsync(userFrom.Id, state.RestorationStep,
            cancellationToken);

        await botClient.SendMessage(userFrom.Id, "Вы успешно изменили свой этап реабилитации", ParseMode.Html,
            cancellationToken: cancellationToken);
    }
}
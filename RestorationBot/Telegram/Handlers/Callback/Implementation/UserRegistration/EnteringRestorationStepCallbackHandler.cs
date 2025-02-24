namespace RestorationBot.Telegram.Handlers.Callback.Implementation.UserRegistration;

using System.Text.RegularExpressions;
using global::Telegram.Bot;
using global::Telegram.Bot.Types;
using RestorationBot.Helpers.Abstract;
using RestorationBot.Shared.Enums;
using RestorationBot.Telegram.FinalStateMachine.OperationsConfiguration.OperationStatesProfiles.UserRegistration;
using RestorationBot.Telegram.FinalStateMachine.States.Implementation;
using RestorationBot.Telegram.FinalStateMachine.StateStorage.Particular.Abstract.Certain;
using RestorationBot.Telegram.Handlers.Callback.Abstract;

public class EnteringRestorationStepCallbackHandler : ICallbackHandler
{
    private readonly ICallbackGenerator _callbackGenerator;
    private readonly IUserRegistrationStateStorageService _particularStateStorageService;

    public EnteringRestorationStepCallbackHandler(ICallbackGenerator callbackGenerator, IUserRegistrationStateStorageService particularStateStorageService)
    {
        _callbackGenerator = callbackGenerator;
        _particularStateStorageService = particularStateStorageService;
    }

    public bool CanHandle(CallbackQuery callbackQuery)
    {
        Match match = _callbackGenerator.GetCallbackRegexOnChoosingRestorationStep().Match(callbackQuery.Data!);
        UserRegistrationState userRegistrationState = _particularStateStorageService.GetOrAddState(callbackQuery.From.Id);

        return match.Success && userRegistrationState.StateMachine.State ==
            UserRegistrationStateProfile.RestorationStepChoosing;
    }

    public async Task HandleCommandAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        Match match = _callbackGenerator.GetCallbackRegexOnChoosingRestorationStep().Match(callbackQuery.Data!);
        if (!match.Success) throw new ArgumentException("Invalid callback query");
        
        RestorationSteps restorationStep =
            (RestorationSteps) int.Parse(match.Groups["index"].Value);
            
        await OnChoosingRestorationStepAsync(restorationStep, callbackQuery.From, botClient, cancellationToken);
    }

    private async Task OnChoosingRestorationStepAsync(RestorationSteps restorationStep, User userFrom, ITelegramBotClient botClient, CancellationToken cancellationToken)
    {
        UserRegistrationState state = _particularStateStorageService.GetOrAddState(userFrom.Id);
        state.RestorationStep = restorationStep;
        
        await state.StateMachine.FireAsync(UserRegistrationTriggerProfile.RestorationStepEntered, cancellationToken);

        await botClient.SendMessage(userFrom.Id, "Введите ваш возраст", cancellationToken: cancellationToken);
    }
}
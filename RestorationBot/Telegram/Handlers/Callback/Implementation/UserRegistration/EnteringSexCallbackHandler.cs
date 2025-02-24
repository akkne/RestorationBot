namespace RestorationBot.Telegram.Handlers.Callback.Implementation.UserRegistration;

using System.Text.RegularExpressions;
using global::Telegram.Bot;
using global::Telegram.Bot.Types;
using RestorationBot.Helpers.Abstract;
using RestorationBot.Services.Abstract;
using RestorationBot.Services.Contracts;
using RestorationBot.Shared.Enums;
using RestorationBot.Telegram.FinalStateMachine.OperationsConfiguration.OperationStatesProfiles.UserRegistration;
using RestorationBot.Telegram.FinalStateMachine.States.Implementation;
using RestorationBot.Telegram.FinalStateMachine.StateStorage.Particular.Abstract.Certain;
using RestorationBot.Telegram.Handlers.Callback.Abstract;
using BusinessUser = Models.User;

public class EnteringSexCallbackHandler : ICallbackHandler
{
    private readonly ICallbackGenerator _callbackGenerator;
    private readonly IUserRegistrationStateStorageService _particularStateStorageService;
    private readonly IUserRegistrationService _userRegistrationService;
    
    public EnteringSexCallbackHandler(ICallbackGenerator callbackGenerator, IUserRegistrationStateStorageService particularStateStorageService, IUserRegistrationService userRegistrationService)
    {
        _callbackGenerator = callbackGenerator;
        _particularStateStorageService = particularStateStorageService;
        _userRegistrationService = userRegistrationService;
    }

    public bool CanHandle(CallbackQuery callbackQuery)
    {
        Match match = _callbackGenerator.GetCallbackRegexOnChoosingSex().Match(callbackQuery.Data!);
        UserRegistrationState userRegistrationState = _particularStateStorageService.GetOrAddState(callbackQuery.From.Id);

        return match.Success && userRegistrationState.StateMachine.State ==
            UserRegistrationStateProfile.SexChoosing;
    }

    public async Task HandleCommandAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        Match match = _callbackGenerator.GetCallbackRegexOnChoosingSex().Match(callbackQuery.Data!);
        if (!match.Success) throw new ArgumentException("Invalid callback query");

        Sex sex = (Sex)int.Parse(match.Groups["index"].Value);
        await OnChoosingSexAsync(sex, botClient, callbackQuery.From, cancellationToken);
    }

    private async Task OnChoosingSexAsync(Sex sex, ITelegramBotClient botClient, User userFrom, CancellationToken cancellationToken)
    {
        UserRegistrationState state = _particularStateStorageService.GetOrAddState(userFrom.Id);
        state.Gender = sex;
        
        await state.StateMachine.FireAsync(UserRegistrationTriggerProfile.SexEntered, cancellationToken);

        UserRegistrationContract contract =
            new UserRegistrationContract(state.UserId, state.Gender, state.Age, state.RestorationStep);
        BusinessUser? user = await _userRegistrationService.RegisterUserAsync(contract, cancellationToken);
        if (user == null)
        {
            throw new Exception($"Failed to register user {userFrom.Id}.");
        }

        const string responseOnSuccessfulRegistration = """
                                                        Вы были успешно зарегистрированы.
                                                        """;
        await botClient.SendMessage(userFrom.Id, responseOnSuccessfulRegistration, cancellationToken: cancellationToken);

    }
}
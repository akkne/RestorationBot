namespace RestorationBot.Telegram.Handlers.Command.Implementation;

using Abstract;
using FinalStateMachine.OperationsConfiguration.OperationStatesProfiles.UserTraining;
using FinalStateMachine.States.Implementation;
using FinalStateMachine.StateStorage.Particular.Abstract.Certain;
using global::Telegram.Bot;
using global::Telegram.Bot.Types;
using RestorationBot.Services.Abstract;
using BusinessUser = Models.User;

public class StartTrainingCommandHandler : ICommandHandler
{
    private const string BaseCommand = "/training";
    private readonly IUserTrainingStateStorageService _storageService;

    private readonly IUserRegistrationService _userRegistrationService;

    public StartTrainingCommandHandler(IUserRegistrationService userRegistrationService,
                                       IUserTrainingStateStorageService storageService)
    {
        _userRegistrationService = userRegistrationService;
        _storageService = storageService;
    }

    public bool CanHandle(string command)
    {
        return command == BaseCommand;
    }

    public async Task HandleCommandAsync(string args, Message message, ITelegramBotClient botClient,
                                         CancellationToken cancellationToken)
    {
        UserTrainingState state = _storageService.GetOrAddState(message.From!.Id);
        if (state.StateMachine.State != UserTrainingStateProfile.Ready)
        {
            _storageService.TryRemove(message.From!.Id);
            _storageService.GetOrAddState(message.From!.Id);
        }

        await state.StateMachine.FireAsync(UserTrainingTriggerProfile.Begin, cancellationToken);

        BusinessUser? user = await _userRegistrationService.GetByTelegramIdAsync(message.From.Id, cancellationToken);
        if (user == null)
        {
            await botClient.SendMessage(message.From.Id,
                "Перед там, как начать тренировку, пройдите регистрацию с помощью команды /start",
                cancellationToken: cancellationToken);
            return;
        }

        const string messageOnGettingHeartRate = """
                                                 Какой у вас показатель частоты сердечных сокращений? (перед выполнением упражнений)
                                                 """;

        await botClient.SendMessage(message.From.Id, messageOnGettingHeartRate,
            cancellationToken: cancellationToken);
    }
}
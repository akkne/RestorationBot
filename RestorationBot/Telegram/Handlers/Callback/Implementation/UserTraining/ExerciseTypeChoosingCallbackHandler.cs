namespace RestorationBot.Telegram.Handlers.Callback.Implementation.UserTraining;

using System.Text.RegularExpressions;
using Abstract;
using FinalStateMachine.OperationsConfiguration.OperationStatesProfiles.UserTraining;
using FinalStateMachine.States.Implementation;
using FinalStateMachine.StateStorage.Particular.Abstract.Certain;
using global::Telegram.Bot;
using global::Telegram.Bot.Types;
using global::Telegram.Bot.Types.Enums;
using global::Telegram.Bot.Types.ReplyMarkups;
using Helpers.Abstract;
using Helpers.Contracts;
using Shared.Enums;

public class ExerciseTypeChoosingCallbackHandler : ICallbackHandler
{
    private readonly ICallbackGenerator _callbackGenerator;
    private readonly IUserTrainingStateStorageService _storageService;
    private readonly IMessageTextGenerator _messageTextGenerator;
    
    public ExerciseTypeChoosingCallbackHandler(ICallbackGenerator callbackGenerator, IUserTrainingStateStorageService storageService, IMessageTextGenerator messageTextGenerator)
    {
        _callbackGenerator = callbackGenerator;
        _storageService = storageService;
        _messageTextGenerator = messageTextGenerator;
    }

    public bool CanHandle(CallbackQuery callbackQuery)
    {
        Match match = _callbackGenerator.GetCallbackRegexOnGetExercise().Match(callbackQuery.Data!);
        UserTrainingState state = _storageService.GetOrAddState(callbackQuery.From.Id);

        return match.Success &&
               state.StateMachine.State == UserTrainingStateProfile.ExerciseTypeChoosing;
    }

    public async Task HandleCommandAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        Match match = _callbackGenerator.GetCallbackRegexOnGetExercise().Match(callbackQuery.Data!);
        if (!match.Success) throw new ArgumentException("Invalid callback query");
        
        RestorationSteps restorationStep =
            (RestorationSteps) int.Parse(match.Groups["step"].Value);
        int exerciseStep = int.Parse(match.Groups["index"].Value);
        
        ExerciseMessageInformation message = ExerciseMessageInformation.Create(restorationStep, exerciseStep);
        await OnGettingExerciseAsync(callbackQuery.From, message, botClient, cancellationToken);
    }
    
    private async Task OnGettingExerciseAsync(User userFrom, ExerciseMessageInformation result, ITelegramBotClient botClient, CancellationToken cancellationToken)
    {
        string messageText = _messageTextGenerator.GenerateExerciseMessageText(result);
        await botClient.SendMessage(userFrom.Id, messageText, ParseMode.Html, cancellationToken: cancellationToken);

        UserTrainingState state = _storageService.GetOrAddState(userFrom.Id);
        state.ExerciseTypeChosen = result.ExerciseIndex;

        const string messageOnGettingHeartRate = """
                                                 Каким был ваш показатель частоты сердечных сокращений после выполнения тренировки?
                                                 """;
        await botClient.SendMessage(userFrom.Id, messageOnGettingHeartRate, ParseMode.Html, cancellationToken: cancellationToken);
        await state.StateMachine.FireAsync(UserTrainingTriggerProfile.ExerciseTypeChosen);
    }
}
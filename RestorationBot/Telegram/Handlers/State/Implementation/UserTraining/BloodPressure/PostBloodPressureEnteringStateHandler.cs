namespace RestorationBot.Telegram.Handlers.State.Implementation.UserTraining.BloodPressure;

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
using Models;
using Models.DataModels;
using RestorationBot.Services.Abstract;
using RestorationBot.Services.Contracts;

public class PostBloodPressureEnteringStateHandler : IStateHandler
{
    private readonly ICallbackGenerator _callbackGenerator;
    private readonly ILogger<PostBloodPressureEnteringStateHandler> _logger;
    private readonly IUserTrainingService _userTrainingService;
    private readonly IUserTrainingStateStorageService _userTrainingStateStorageService;

    public PostBloodPressureEnteringStateHandler(IUserTrainingStateStorageService userTrainingStateStorageService,
                                                 IUserTrainingService userTrainingService,
                                                 ICallbackGenerator callbackGenerator,
                                                 ILogger<PostBloodPressureEnteringStateHandler> logger)
    {
        _userTrainingStateStorageService = userTrainingStateStorageService;
        _userTrainingService = userTrainingService;
        _callbackGenerator = callbackGenerator;
        _logger = logger;
    }

    public bool CanHandle(Message message)
    {
        UserTrainingState state = _userTrainingStateStorageService.GetOrAddState(message.From!.Id);
        return state.StateMachine.State == UserTrainingStateProfile.PostBloodPressureEntering;
    }

    public async Task HandleStateAsync(ITelegramBotClient botClient, Message message,
                                       CancellationToken cancellationToken)
    {
        if (message.Text == null) throw new NullReferenceException("Message is null");

        UserTrainingState state = _userTrainingStateStorageService.GetOrAddState(message.From!.Id);

        string bloodPressureText = message.Text.Trim();
        _logger.LogInformation("Blood pressure text is: {text}", bloodPressureText);

        if (!IsValidBloodPressure(bloodPressureText))
            throw new ArgumentException("Invalid blood pressure provided");
        state.PostBloodPressure = bloodPressureText;

        await state.StateMachine.FireAsync(UserTrainingTriggerProfile.PostBloodPressureEntered, cancellationToken);

        UserTrainingReportingContract contract = new(message.From!.Id,
            new TrainingReportData
            {
                BloodPressure = state.PreBloodPressure,
                HeartRate = state.PreHeartRate
            },
            new TrainingReportData
            {
                BloodPressure = state.PostBloodPressure,
                HeartRate = state.PostHeartRate
            });
        TrainingReport? report = await _userTrainingService.ReportUserTrainingAsync(contract, cancellationToken);
        if (report == null) throw new Exception("Failed to report user training data");

        const string messageOnSuccessfulReport = """
                                                 Данные о вашей тренировке были успешно сохранены, для того, что посмотреть их вы можете воспользоваться /reports.
                                                 Возникли какие-то проблемы при выполнении упражнений?
                                                 """;

        InlineKeyboardMarkup keyboardMarkup = new(new List<InlineKeyboardButton>
            {
                new("Да")
                {
                    CallbackData = _callbackGenerator.GenerateCallbackOnHavingProblem(true)
                },
                new("Нет")
                {
                    CallbackData = _callbackGenerator.GenerateCallbackOnHavingProblem(false)
                }
            }
        );

        await botClient.SendMessage(message.From!.Id, messageOnSuccessfulReport, replyMarkup: keyboardMarkup,
            parseMode: ParseMode.Html, cancellationToken: cancellationToken);
    }

    private static bool IsValidBloodPressure(string bloodPressure)
    {
        string pattern = @"^\d+/\d+$";
        return Regex.IsMatch(bloodPressure, pattern);
    }
}
namespace RestorationBot.Telegram.Handlers.State.Implementation.UserTraining;

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

public class BloodPressureEnteringStateHandler : IStateHandler
{
    private readonly ICallbackGenerator _callbackGenerator;
    private readonly IUserTrainingService _userTrainingService;
    private readonly IUserTrainingStateStorageService _userTrainingStateStorageService;

    public BloodPressureEnteringStateHandler(IUserTrainingStateStorageService userTrainingStateStorageService,
                                             IUserTrainingService userTrainingService,
                                             ICallbackGenerator callbackGenerator)
    {
        _userTrainingStateStorageService = userTrainingStateStorageService;
        _userTrainingService = userTrainingService;
        _callbackGenerator = callbackGenerator;
    }

    public bool CanHandle(Message message)
    {
        UserTrainingState state = _userTrainingStateStorageService.GetOrAddState(message.From!.Id);
        return state.StateMachine.State == UserTrainingStateProfile.BloodPressureEntering;
    }

    public async Task HandleStateAsync(ITelegramBotClient botClient, Message message,
                                       CancellationToken cancellationToken)
    {
        UserTrainingState state = _userTrainingStateStorageService.GetOrAddState(message.From!.Id);

        if (!double.TryParse(message.Text, out double bloodPressure))
            throw new ArgumentException("Invalid blood pressure provided");
        state.BloodPressure = bloodPressure;

        await state.StateMachine.FireAsync(UserTrainingTriggerProfile.BloodPressureEntered, cancellationToken);

        UserTrainingReportingContract contract = new(message.From!.Id,
            new TrainingReportData
            {
                BloodPressure = state.BloodPressure,
                HeartRate = state.HeartRate
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
}
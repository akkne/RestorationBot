namespace RestorationBot.Telegram.Handlers.Command.Implementation;

using Abstract;
using global::Telegram.Bot;
using global::Telegram.Bot.Types;
using global::Telegram.Bot.Types.Enums;
using Models;
using RestorationBot.Services.Abstract;

public class GetTrainingReportsCommandHandler : ICommandHandler
{
    private const string BaseCommandName = "/reports";
    
    private readonly IUserTrainingService _userTrainingService;

    public GetTrainingReportsCommandHandler(IUserTrainingService userTrainingService)
    {
        _userTrainingService = userTrainingService;
    }

    public bool CanHandle(string command)
    {
        return command == BaseCommandName;
    }

    public async Task HandleCommandAsync(string args, Message message, ITelegramBotClient botClient,
                                         CancellationToken cancellationToken)
    {
        List<TrainingReport> reports = await _userTrainingService.GetUserTrainingReportsAsync(message.From!.Id, cancellationToken);
        if (reports.Count == 0)
        {
            const string messageOnNoReports = """
                                              У вас еще не было проведено ни одной тренировки.
                                              """;
            await botClient.SendMessage(message.From.Id, messageOnNoReports, cancellationToken: cancellationToken);
            return;
        }

        for (int index = 0; index < reports.Count; index++)
        {
            string reportMessage = GenerateReportMessage(reports[index], index + 1);
            await botClient.SendMessage(message.From.Id, reportMessage, parseMode: ParseMode.Html, cancellationToken: cancellationToken);
        }
    }

    private string GenerateReportMessage(TrainingReport report, int index)
    {
        return $"""
                {index}. Тренировка от {report.TrainingDate.ToShortTimeString()} {report.TrainingDate.ToShortDateString()}

                <strong>Частота сердечных сокращений</strong>: {report.TrainingReportData.HeartRate}
                <strong>Артеривальное давление</strong>: {report.TrainingReportData.BloodPressure}
                """;
    }
}
namespace RestorationBot.Telegram.Handlers.Command.Implementation;

using Abstract;
using global::Telegram.Bot;
using global::Telegram.Bot.Types;
using global::Telegram.Bot.Types.Enums;
using Models;
using RestorationBot.Services.Abstract;

public class GetPainReportsCommandHandler : ICommandHandler
{
    private const string BaseCommandName = "/pain_reports";

    private readonly IPainReportService _painReportService;

    public GetPainReportsCommandHandler(IPainReportService painReportService)
    {
        _painReportService = painReportService;
    }

    public bool CanHandle(string command)
    {
        return command == BaseCommandName;
    }

    public async Task HandleCommandAsync(string args, Message message, ITelegramBotClient botClient,
                                         CancellationToken cancellationToken)
    {
        List<PainReport> reports =
            await _painReportService.GetUserPainReportsAsync(message.From!.Id, cancellationToken);
        if (reports.Count == 0)
        {
            const string messageOnNoReports = """
                                              У вас еще не было записано случаев, когда была боль.
                                              """;
            await botClient.SendMessage(message.From.Id, messageOnNoReports, cancellationToken: cancellationToken);
            return;
        }

        for (int index = 0; index < reports.Count; index++)
        {
            string reportMessage = GenerateReportMessage(reports[index], index + 1);
            await botClient.SendMessage(message.From.Id, reportMessage, ParseMode.Html,
                cancellationToken: cancellationToken);
        }
    }

    private string GenerateReportMessage(PainReport report, int index)
    {
        return $"""
                {index}. Тренировка от {report.ReportDate.ToString("HH:mm dd/MM/yyyy")}

                <strong>Уровень боли:</strong> {report.PainLevel}
                """;
    }
}
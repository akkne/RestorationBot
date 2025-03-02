namespace RestorationBot.Telegram.Handlers.State.Implementation.HavingPain;

using Abstract;
using FinalStateMachine.OperationsConfiguration.OperationStatesProfiles.HavingPain;
using FinalStateMachine.States.Implementation;
using FinalStateMachine.StateStorage.Particular.Abstract.Certain;
using global::Telegram.Bot;
using global::Telegram.Bot.Types;
using global::Telegram.Bot.Types.Enums;
using Models;
using RestorationBot.Services.Abstract;
using RestorationBot.Services.Contracts;

public class PainLevelEnteringStateHandler : IStateHandler
{
    private readonly IPainReportService _painReportService;
    private readonly IUserHavingPainStateStorageService _stateStorageService;

    public PainLevelEnteringStateHandler(IUserHavingPainStateStorageService stateStorageService,
                                         IPainReportService painReportService)
    {
        _stateStorageService = stateStorageService;
        _painReportService = painReportService;
    }

    public bool CanHandle(Message message)
    {
        HavingPainState state = _stateStorageService.GetOrAddState(message.From!.Id);
        return state.StateMachine.State == HavingPainStateProfile.PainValueEntering;
    }

    public async Task HandleStateAsync(ITelegramBotClient botClient, Message message,
                                       CancellationToken cancellationToken)
    {
        HavingPainState state = _stateStorageService.GetOrAddState(message.From!.Id);

        if (!int.TryParse(message.Text, out int painLevel)) throw new ArgumentException("Invalid pain level");
        if (painLevel is < 0 or > 10) throw new ArgumentException("Invalid pain level: has to be between 0 and 10");

        PainReport? result =
            await _painReportService.ReportUserPainAsync(new UserPainRetortingContract(message.From!.Id, painLevel),
                cancellationToken);
        if (result == null) throw new Exception("Something went wrong with pain reporting");

        const string messageOnSuccessfulPainReport = """
                                                     Ваши данные были успешно сохранены. 
                                                     Для того, чтобы получить их, воспользуйтесь командой /pain_reports.
                                                     """;
        await botClient.SendMessage(message.From!.Id, messageOnSuccessfulPainReport, ParseMode.Html,
            cancellationToken: cancellationToken);
        await state.StateMachine.FireAsync(HavingPainTriggerProfile.PainValueEntered);
    }
}
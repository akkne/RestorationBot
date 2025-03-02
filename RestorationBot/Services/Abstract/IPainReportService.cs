namespace RestorationBot.Services.Abstract;

using Contracts;
using Models;

public interface IPainReportService
{
    Task<PainReport?> ReportUserPainAsync(UserPainRetortingContract contract,
                                          CancellationToken cancellationToken = default);

    Task<List<PainReport>> GetUserPainReportsAsync(long userTelegramId, CancellationToken cancellationToken = default);
}
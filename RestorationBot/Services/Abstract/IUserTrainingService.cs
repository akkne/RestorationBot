namespace RestorationBot.Services.Abstract;

using Contracts;
using Models;

public interface IUserTrainingService
{
    Task<TrainingReport?> ReportUserTrainingAsync(UserTrainingReportingContract userTrainingReportingContract, CancellationToken cancellationToken = default);
    Task<List<TrainingReport>> GetUserTrainingReportsAsync(long telegramUserId, CancellationToken cancellationToken = default);
}
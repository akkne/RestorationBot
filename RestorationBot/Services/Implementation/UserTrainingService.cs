namespace RestorationBot.Services.Implementation;

using Abstract;
using Contracts;
using Database.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Models;

public class UserTrainingService : IUserTrainingService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<UserTrainingService> _logger;

    public UserTrainingService(ApplicationDbContext dbContext, ILogger<UserTrainingService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<TrainingReport?> ReportUserTrainingAsync(
        UserTrainingReportingContract userTrainingReportingContract, CancellationToken cancellationToken = default)
    {
        try
        {
            await using IDbContextTransaction transaction =
                await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            User sportsmen = await _dbContext.Users
                                             .FirstOrDefaultAsync(
                                                  x => x.TelegramId == userTrainingReportingContract.TelegramUserId,
                                                  cancellationToken)
                          ?? throw new NullReferenceException(
                                 $"User with telegram id: {userTrainingReportingContract.TelegramUserId} not found");

            TrainingReport report = TrainingReport.Create(sportsmen, userTrainingReportingContract.TrainingReportData);

            await _dbContext.TrainingReports.AddAsync(report, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return report;
        }
        catch (NullReferenceException exception)
        {
            _logger.LogError(exception, exception.Message);
            return null;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, exception.Message);
            return null;
        }
    }

    public async Task<List<TrainingReport>> GetUserTrainingReportsAsync(
        long telegramUserId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.TrainingReports
                               .AsNoTracking()
                               .ToListAsync(cancellationToken);
    }
}
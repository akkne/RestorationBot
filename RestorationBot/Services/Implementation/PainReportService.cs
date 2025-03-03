namespace RestorationBot.Services.Implementation;

using Abstract;
using Contracts;
using Database.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Models;

public class PainReportService : IPainReportService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<PainReportService> _logger;

    public PainReportService(ApplicationDbContext dbContext, ILogger<PainReportService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<PainReport?> ReportUserPainAsync(UserPainRetortingContract contract,
                                                       CancellationToken cancellationToken = default)
    {
        try
        {
            await using IDbContextTransaction transaction =
                await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            User? user = await _dbContext.Users
                                         .FirstOrDefaultAsync(x => x.TelegramId == contract.UserTelegramId,
                                              cancellationToken);

            if (user == null) return null;
            PainReport created = PainReport.Create(user, contract.PainLevel);

            await _dbContext.PainReports.AddAsync(created, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return created;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return null;
        }
    }

    public async Task<List<PainReport>> GetUserPainReportsAsync(long userTelegramId,
                                                                CancellationToken cancellationToken = default)
    {
        return await _dbContext.PainReports
                               .AsNoTracking()
                               .Where(x => x.Author.TelegramId == userTelegramId)
                               .OrderBy(x => x.ReportDate)
                               .ToListAsync(cancellationToken);
    }
}
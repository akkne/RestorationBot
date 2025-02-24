namespace RestorationBot.Services.Implementation;

using Abstract;
using Contracts;
using Database.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Shared.Enums;
using User = Models.User;

public class UserRegistrationService : IUserRegistrationService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<UserRegistrationService> _logger;

    public UserRegistrationService(ApplicationDbContext dbContext, ILogger<UserRegistrationService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<User?> RegisterUserAsync(UserRegistrationContract userRegistration, CancellationToken cancellationToken = default)
    {
        User created = User.Create(userRegistration.TelegramId, userRegistration.Age, userRegistration.Gender, userRegistration.RestorationStep);

        try
        {
            await using IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            await _dbContext.Users.AddAsync(created, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to create user");
            return null;
        }

        return created;
    }

    public async Task<bool> ContainsUserAsync(long telegramId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users.AnyAsync(x => x.TelegramId == telegramId, cancellationToken: cancellationToken);
    }

    public async Task<User?> GetByTelegramIdAsync(long telegramId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
                               .AsNoTracking()
                               .FirstOrDefaultAsync(x => x.TelegramId == telegramId, cancellationToken: cancellationToken);
    }

    public async Task UpdateUserRestorationStepAsync(long telegramId, RestorationSteps restorationStep, CancellationToken cancellationToken = default)
    {
        await using IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        await _dbContext.Users.Where(x => x.TelegramId == telegramId)
                        .ExecuteUpdateAsync(
                             setters => setters.SetProperty(x => x.RestorationStep, x => restorationStep), cancellationToken: cancellationToken);
        
        await transaction.CommitAsync(cancellationToken);
    }
}
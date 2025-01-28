namespace RestorationBot.Services.Implementation;

using Abstact;
using Database.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Shared.Enums;
using Telegram.Bot.Types;
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

    public async Task<User?> RegisterUserAsync(long telegramId, RestorationSteps restorationStep)
    {
        User created = User.Create(telegramId, restorationStep);

        try
        {
            await using IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync();

            await _dbContext.Users.AddAsync(created);
            await _dbContext.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to create user");
            return null;
        }

        return created;
    }

    public async Task<bool> ContainsUserAsync(long telegramId)
    {
        return await _dbContext.Users.AnyAsync(x => x.TelegramId == telegramId);
    }

    public async Task<User?> GetByTelegramIdAsync(long telegramId)
    {
        return await _dbContext.Users
                               .AsNoTracking()
                               .FirstOrDefaultAsync(x => x.TelegramId == telegramId);
    }

    public async Task UpdateUserRestorationStepAsync(long telegramId, RestorationSteps restorationStep)
    {
        await using IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync();

        await _dbContext.Users.Where(x => x.TelegramId == telegramId)
                        .ExecuteUpdateAsync(
                             setters => setters.SetProperty(x => x.RestorationStep, x => restorationStep));
        
        await transaction.CommitAsync();
    }
}
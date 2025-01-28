namespace RestorationBot.Services.Abstact;

using Models;
using Shared.Enums;

public interface IUserRegistrationService
{
    Task<User?> RegisterUserAsync(long telegramId, RestorationSteps restorationStep);
    Task<bool> ContainsUserAsync(long telegramId);
    Task<User?> GetByTelegramIdAsync(long telegramId);
    Task UpdateUserRestorationStepAsync(long telegramId, RestorationSteps restorationStep);
}
namespace RestorationBot.Services.Abstract;

using Contracts;
using Models;
using Shared.Enums;

public interface IUserRegistrationService
{
    Task<User?> RegisterUserAsync(UserRegistrationContract userRegistration,
                                  CancellationToken cancellationToken = default);

    Task<bool> ContainsUserAsync(long telegramId, CancellationToken cancellationToken = default);
    Task<User?> GetByTelegramIdAsync(long telegramId, CancellationToken cancellationToken = default);

    Task UpdateUserRestorationStepAsync(long telegramId, RestorationSteps restorationStep,
                                        CancellationToken cancellationToken = default);
}
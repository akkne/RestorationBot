namespace RestorationBot.Services.Contracts;

using Shared.Enums;

public record UserRegistrationContract(long TelegramId, Sex Gender, int Age, RestorationSteps RestorationStep);
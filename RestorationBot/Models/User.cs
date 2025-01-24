namespace RestorationBot.Models;

using Shared.Enums;

public class User
{
    public Guid Id { get; set; }
    public long TelegramId { get; set; }
    public RestorationSteps RestorationStep { get; set; }

    public static User Create(long telegramId, RestorationSteps restorationStep)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            TelegramId = telegramId,
            RestorationStep = restorationStep
        };
    }
}
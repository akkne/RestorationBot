namespace RestorationBot.Models;

using Shared.Enums;

public class User
{
    public Guid Id { get; set; }
    public long TelegramId { get; set; }
    public RestorationSteps RestorationStep { get; set; }
    public List<TrainingReport> TrainingReports { get; set; }
    public List<PainReport> PainReports { get; set; }

    public int Age { get; set; }
    public Sex Sex { get; set; }

    public static User Create(long telegramId, int age, Sex sex, RestorationSteps restorationStep)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            TelegramId = telegramId,
            Age = age,
            Sex = sex,
            RestorationStep = restorationStep,
            TrainingReports = []
        };
    }
}
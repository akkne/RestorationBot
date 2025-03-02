namespace RestorationBot.Models;

using System.ComponentModel.DataAnnotations;

public class PainReport
{
    public Guid Id { get; set; }
    public User Author { get; set; }

    [Range(0, 10)] public int PainLevel { get; set; }

    public DateTime ReportDate { get; set; }

    public static PainReport Create(User user, int painLevel)
    {
        return new PainReport
        {
            Id = Guid.NewGuid(),
            Author = user,
            PainLevel = painLevel,
            ReportDate = DateTime.UtcNow
        };
    }
}
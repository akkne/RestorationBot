namespace RestorationBot.Models;

using DataModels;

public class TrainingReport
{
    public Guid Id { get; set; }
    public User Sportsmen { get; set; }
    public TrainingReportData TrainingReportData { get; set; }
    public DateTime TrainingDate { get; set; }

    public static TrainingReport Create(User sportsmen, TrainingReportData trainingReportData)
    {
        return new TrainingReport()
        {
            Id = Guid.NewGuid(),
            Sportsmen = sportsmen,
            TrainingReportData = trainingReportData,
            TrainingDate = DateTime.UtcNow
        };
    }
}
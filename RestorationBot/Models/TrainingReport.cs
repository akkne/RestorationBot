namespace RestorationBot.Models;

using DataModels;

public class TrainingReport
{
    public Guid Id { get; set; }
    public User Sportsmen { get; set; }
    public TrainingReportData PreTrainingReportData { get; set; }
    public TrainingReportData PostTrainingReportData { get; set; }
    public DateTime TrainingDate { get; set; }

    public static TrainingReport Create(User sportsmen, TrainingReportData preTrainingReportData,
                                        TrainingReportData postTrainingReportData)
    {
        return new TrainingReport
        {
            Id = Guid.NewGuid(),
            Sportsmen = sportsmen,
            PreTrainingReportData = preTrainingReportData,
            PostTrainingReportData = postTrainingReportData,
            TrainingDate = DateTime.UtcNow
        };
    }
}
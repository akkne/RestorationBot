namespace RestorationBot.Services.Contracts;

using Models.DataModels;

public record UserTrainingReportingContract(long TelegramUserId, TrainingReportData TrainingReportData);

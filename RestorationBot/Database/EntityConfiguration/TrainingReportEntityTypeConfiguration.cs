namespace RestorationBot.Database.EntityConfiguration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models;

public class TrainingReportEntityTypeConfiguration : IEntityTypeConfiguration<TrainingReport>
{
    public void Configure(EntityTypeBuilder<TrainingReport> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.OwnsOne(x => x.PreTrainingReportData, reportData =>
        {
            reportData.Property(x => x.BloodPressure)
                      .IsRequired();

            reportData.Property(x => x.HeartRate)
                      .IsRequired();
        });

        builder.OwnsOne(x => x.PostTrainingReportData, reportData =>
        {
            reportData.Property(x => x.BloodPressure)
                      .IsRequired();

            reportData.Property(x => x.HeartRate)
                      .IsRequired();
        });
    }
}
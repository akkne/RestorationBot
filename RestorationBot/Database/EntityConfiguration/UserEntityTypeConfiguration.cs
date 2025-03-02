namespace RestorationBot.Database.EntityConfiguration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models;
using Shared.Enums;

public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.TelegramId).ValueGeneratedNever();

        builder.Property(x => x.Sex)
               .HasConversion(
                    v => v.ToString(),
                    v => Enum.Parse<Sex>(v)
                )
               .IsRequired();

        builder.Property(x => x.RestorationStep)
               .HasConversion(
                    v => v.ToString(),
                    v => Enum.Parse<RestorationSteps>(v)
                )
               .IsRequired();

        builder.HasMany(x => x.TrainingReports)
               .WithOne(x => x.Sportsmen);

        builder.HasMany(x => x.PainReports)
               .WithOne(x => x.Author);
    }
}
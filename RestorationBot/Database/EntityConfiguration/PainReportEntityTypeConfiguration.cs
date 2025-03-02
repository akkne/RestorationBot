namespace RestorationBot.Database.EntityConfiguration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models;

public class PainReportEntityTypeConfiguration : IEntityTypeConfiguration<PainReport>
{
    public void Configure(EntityTypeBuilder<PainReport> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();
    }
}
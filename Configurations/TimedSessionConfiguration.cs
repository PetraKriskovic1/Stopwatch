using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stopwatch.Domain;

namespace Stopwatch.Configurations;

public class TimeTrackingConfiguration : IEntityTypeConfiguration<TimedSession>
{
    public void Configure(EntityTypeBuilder<TimedSession> builder)
    {
        builder.HasKey(t => t.Id);
        builder.HasOne(t => t.Participant); 
    }
}
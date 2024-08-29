using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stopwatch.Domain;

namespace Stopwatch.Configurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.Property(e => e.RoleName).HasMaxLength(200);

        builder.HasDiscriminator<string>("RoleName")
            .HasValue<Administrator>("Administrator")
            .HasValue<Participant>("Participant");
        
        builder.HasOne(t => t.User); 
    }
}
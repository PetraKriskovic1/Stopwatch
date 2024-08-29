using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stopwatch.Domain;

namespace Stopwatch.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.FirstName).HasMaxLength(200);
        builder.Property(e => e.LastName).HasMaxLength(200);
        builder.Property(e => e.EmailAddress).HasMaxLength(200);
        builder.Property(e => e.Password).HasMaxLength(200);

        builder.HasMany(u => u.Roles);
    }
}
using FitnessTracker.Domain.Aggregates;
using FitnessTracker.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitnessTracker.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => new UserId(value))
            .ValueGeneratedNever();

        builder.Property(x => x.TelegramChatId)
            .IsRequired();

        builder.Property(x => x.TelegramUsername)
            .HasMaxLength(100);

        builder.Property(x => x.Timezone)
            .HasMaxLength(50)
            .IsRequired();
    }
}

using InovaBank.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InovaBank.Infrastructure.Persistence.Configurations;

public sealed class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).ValueGeneratedNever();

        builder.Property(t => t.Amount).HasPrecision(18, 2).IsRequired();
        builder.Property(t => t.Currency).HasMaxLength(3).IsRequired();
        builder.Property(t => t.Type).HasConversion<int>().IsRequired();
        builder.Property(t => t.Description).HasMaxLength(100);
        builder.Property(t => t.CreatedAt).IsRequired();

        builder.HasOne<Account>()
            .WithMany(a => a.Transactions)
            .HasForeignKey(t => t.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(t => new { t.AccountId, t.CreatedAt })
            .HasDatabaseName("IX_Transactions_AccountId_CreatedAt");

        builder.HasIndex(t => new { t.AccountId, t.Type })
            .HasDatabaseName("IX_Transactions_AccountId_Type");

        builder.HasIndex(t => t.CreatedAt);
    }
}

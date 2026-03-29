using InovaBank.Domain.Entities;
using InovaBank.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InovaBank.Infrastructure.Persistence.Configurations;

public sealed class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Cnpj)
            .HasConversion(v => v.Number, v => new Cnpj(v))
            .IsRequired()
            .HasMaxLength(14);

        builder.HasIndex(a => a.Cnpj).IsUnique();

        builder.Property(a => a.RazaoSocial).IsRequired().HasMaxLength(200);
        builder.Property(a => a.ImagemDocumentoPath).IsRequired().HasMaxLength(250);
        builder.Property(a => a.Agencia).IsRequired().HasMaxLength(4);
        builder.Property(a => a.Balance).HasPrecision(18, 2);
        builder.Property(a => a.Status).HasConversion<string>();
    }
}

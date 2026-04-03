using InovaBank.Domain.Entities;
using InovaBank.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using MassTransit;

namespace InovaBank.Infrastructure.Persistence;

public sealed class InovaBankDbContext(DbContextOptions<InovaBankDbContext> options)
    : DbContext(options), IUnitOfWork
{
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.AddTransactionalOutboxEntities();

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InovaBankDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}

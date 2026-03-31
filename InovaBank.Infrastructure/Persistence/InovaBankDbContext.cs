using InovaBank.Domain.Entities;
using InovaBank.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InovaBank.Infrastructure.Persistence;

public sealed class InovaBankDbContext(DbContextOptions<InovaBankDbContext> options)
    : DbContext(options), IUnitOfWork
{
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InovaBankDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}

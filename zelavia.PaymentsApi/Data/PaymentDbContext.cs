using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;

namespace zelavia.PaymentsApi.Data;

public class PaymentDbContext : DbContext
{
    public DbSet<Payment> Payments { get; init; } = null!;

    public static PaymentDbContext Create(IMongoDatabase database) =>
       new(new DbContextOptionsBuilder<PaymentDbContext>()
           .UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName)
           .Options);

    public PaymentDbContext(DbContextOptions options)
        : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Payment>().ToCollection("payments");
    }
}

using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using zelavia.FlightBookingApi.Sagas;

namespace zelavia.FlightBookingApi.Data;

public class BookingDbContext : SagaDbContext
{
    public BookingDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override IEnumerable<ISagaClassMap> Configurations
    {
        get
        {
            yield return new BookingStateMap();
        }
    }
}

public class BookingStateMap : SagaClassMap<BookingState>
{
    protected override void Configure(EntityTypeBuilder<BookingState> entity, ModelBuilder model)
    {
        entity.Property(x => x.CurrentState).HasMaxLength(64);
        entity.Property(x => x.UserEmail).HasMaxLength(256);
        entity.Property(x => x.Amount);
    }
}

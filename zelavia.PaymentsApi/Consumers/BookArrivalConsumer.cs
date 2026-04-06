using MassTransit;
using zelavia.Contracts.Messages;
using zelavia.PaymentsApi.Data;

namespace zelavia.PaymentsApi.Consumers;

public class BookingCreatedConsumer(PaymentDbContext dbContext, ILogger<BookingCreatedConsumer> logger) : IConsumer<BookingCreated>
{
    readonly PaymentDbContext _dbContext = dbContext;
    readonly ILogger<BookingCreatedConsumer> _logger = logger;

    public async Task Consume(ConsumeContext<BookingCreated> context)
    {
        _logger.LogInformation("Creating Booking request {BookingId}", context.Message.BookingId);

        await _dbContext.Payments.AddAsync(new Payment(context.Message.BookingId, context.Message.UserEmail, context.Message.Amount));
        await _dbContext.SaveChangesAsync();

        await context.Publish(new PaymentConfirmed(context.Message.BookingId, context.Message.UserEmail, context.Message.Amount));
    }
}

using MassTransit;
using zelavia.Contracts.Messages;
using zelavia.PaymentsApi.Data;

namespace zelavia.PaymentsApi.Consumers;

public class ProcessPaymentConsumer(
    IPaymentService paymentService,
    PaymentDbContext dbContext,
    ILogger<ProcessPaymentConsumer> logger) : IConsumer<ProcessPayment>
{
    public async Task Consume(ConsumeContext<ProcessPayment> context)
    {
        logger.LogInformation("Creating ProcessPaymentConsumer request {BookingId}", context.Message.BookingId);

        try
        {
            var payment = await paymentService.ProcessPaymentAsync(
                bookingId: context.Message.BookingId,
                userId: context.Message.UserId,
                amount: context.Message.Amount,
                paymentIntentId: context.Message.PaymentIntentId);

            if (payment is not null)
            {
                await context.Publish<PaymentConfirmed>(new
                {
                    BookingId = context.Message.BookingId,
                    PaymentIntentId = payment.PaymentIntentId
                });

                await dbContext.Payments.AddAsync(payment);
                await dbContext.SaveChangesAsync();
            }
            else
            {
                await context.Publish<PaymentFailed>(new
                {
                    BookingId = context.Message.BookingId,
                    Reason = "Payment proccesing error"
                });
            }

        }
        catch (Exception ex)
        {
            logger.LogError(
                            ex,
                            "Failed to process payment for booking {BookingId}",
                            context.Message.BookingId);

            await context.Publish<PaymentFailed>(new
            {
                BookingId = context.Message.BookingId,
                Reason = "Payment processing error"
            });
        }
    }
}

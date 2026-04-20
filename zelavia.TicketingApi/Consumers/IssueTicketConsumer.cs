using MassTransit;
using zelavia.Contracts.Messages;

namespace zelavia.TicketingApi.Consumers;

public class IssueTicketConsumer(
    EmailService emailService,
    ILogger<IssueTicketConsumer> logger) : IConsumer<IssueTicket>
{
    public async Task Consume(ConsumeContext<IssueTicket> context)
    {
        logger.LogInformation("IssueTicket request {UserEmail}", context.Message.UserEmail);

        try
        {
            await emailService.SendEmailAsync(context.Message.UserEmail, "test", "test");
        }
        catch (Exception ex)
        {
            logger.LogError(
                    ex,
                    "Failed to process ticket for booking {BookingId}",
                    context.Message.BookingId);

            await context.Publish<BookingFailed>(new
            {
                BookingId = context.Message.BookingId,
                Reason = "ticket processing error"
            })
        ;
        }
    }
}

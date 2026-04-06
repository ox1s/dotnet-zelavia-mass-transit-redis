using zelavia.PaymentsApi.Data;
using static MongoDB.Driver.WriteConcern;

namespace zelavia.PaymentsApi;

public class PaymentService : IPaymentService
{
    public Task<Payment?> ProcessPaymentAsync(Guid bookingId, Guid userId, decimal amount, string paymentIntentId)
    {
        return Task.FromResult<Payment?>(new Payment(bookingId, userId, amount, paymentIntentId, DateTime.UtcNow));
    }
}

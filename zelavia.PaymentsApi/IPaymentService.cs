using zelavia.PaymentsApi.Data;

namespace zelavia.PaymentsApi;

public interface IPaymentService
{
    Task<Payment?> ProcessPaymentAsync(Guid bookingId, Guid userId, decimal amount, string paymentIntentId);
}

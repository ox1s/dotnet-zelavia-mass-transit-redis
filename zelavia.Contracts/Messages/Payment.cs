namespace zelavia.Contracts.Messages;

public record ProcessPayment(Guid BookingId, Guid UserId, decimal Amount, string PaymentIntentId);
public record PaymentConfirmed(Guid BookingId, string PaymentIntentId);
public record PaymentFailed(Guid BookingId, string UserEmail, decimal Amount);
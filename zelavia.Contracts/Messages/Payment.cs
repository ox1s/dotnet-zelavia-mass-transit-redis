namespace zelavia.Contracts.Messages;

public record ProcessPayment(Guid BookingId, string UserEmail, decimal Amount);
public record PaymentConfirmed(Guid BookingId, string UserEmail, decimal Amount);
public record PaymentFailed(Guid BookingId, string UserEmail, decimal Amount);
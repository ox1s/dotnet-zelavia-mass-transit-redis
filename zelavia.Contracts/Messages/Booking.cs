namespace zelavia.Contracts.Messages;

public record BookingCreated(Guid BookingId, string UserEmail, decimal Amount);
public record BookingConfirmed(Guid BookingId, string UserEmail, decimal Amount);
public record BookingFailed(Guid BookingId, string UserEmail, decimal Amount);
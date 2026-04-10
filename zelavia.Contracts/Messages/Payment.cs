namespace zelavia.Contracts.Messages;

public record ProcessPayment
{
    public Guid BookingId { get; init; }
    public Guid UserId { get; init; }
    public decimal Amount { get; init; }
    public string PaymentIntentId { get; init; }
}

public record PaymentConfirmed
{
    public Guid BookingId { get; init; }
    public string PaymentIntentId { get; init; }
}
public record PaymentFailed
{
    public Guid BookingId { get; init; }
    public string Reason { get; init; }
}
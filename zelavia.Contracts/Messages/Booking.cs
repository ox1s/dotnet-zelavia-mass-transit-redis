namespace zelavia.Contracts.Messages;

public record BookingCreated
{
    public Guid BookingId { get; init; }
    public Guid UserId { get; init; }
    public string UserEmail { get; init; } = null!;
    public decimal Amount { get; init; }
    public DateTime BookingDateUtc { get; init; }
}
public record BookingConfirmed
{
    public Guid BookingId { get; init; }
    public string UserEmail { get; init; } = null!;
    public decimal Amount { get; init; }
}
public record BookingFailed
{
    public Guid BookingId { get; init; }
    public string Reason { get; init; } = null!;
}

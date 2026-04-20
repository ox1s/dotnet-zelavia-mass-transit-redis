namespace zelavia.Contracts.Messages;

public record IssueTicket
{
    public Guid BookingId { get; init; }
    public string UserEmail { get; init; } = null!;
}

public record TicketIssued
{
    public Guid BookingId { get; init; }
    public string UserEmail { get; init; } = null!;
}

public record TicketFailed
{
    public Guid BookingId { get; init; }
    public string Reason { get; init; } = null!;
}

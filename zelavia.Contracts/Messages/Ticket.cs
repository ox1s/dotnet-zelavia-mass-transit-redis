namespace zelavia.Contracts.Messages;

public record IssueTicket(Guid BookingId, string UserEmail);
public record TicketIssued(Guid BookingId, string UserEmail);
public record TicketFailed(Guid BookingId, string Reason);
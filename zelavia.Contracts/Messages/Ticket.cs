namespace zelavia.Contracts.Messages;

public record TicketIssued(Guid BookingId, string UserEmail);
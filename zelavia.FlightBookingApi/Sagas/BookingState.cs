using MassTransit;
using zelavia.Contracts.Messages;

namespace zelavia.FlightBookingApi.Sagas;


public class BookingState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; } = null!;

    public decimal Amount { get; set; }
    public DateTime BookingDateUtc { get; set; }
    public string UserEmail { get; set; } = null!;
    public Guid UserId { get; set; }
}

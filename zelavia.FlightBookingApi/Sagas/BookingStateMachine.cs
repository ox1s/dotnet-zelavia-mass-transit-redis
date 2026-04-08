using MassTransit;
using zelavia.Contracts.Messages;

namespace zelavia.FlightBookingApi.Sagas;

public class BookingStateMachine : MassTransitStateMachine<BookingState>
{
    // Бронь места (Fligh..Api) -> Снятие денег (PaymentApi) -> Отправка билета (TicketsApi)

    // Booking Created -> ProccessPayment -> ProccessingPayment -> PaymentSucceded -> IssueTicket -> Proccessingticket -> TicketIssued -> BookingConfirmed
    //                                                             PaymentFailed -> BookingFailed                       TicketFailed -> BookingFailed
    public Event<BookingCreated> BookingCreated { get; private set; } = null!;

    public Event<PaymentConfirmed> PaymentConfirmed { get; private set; } = null!;
    public Event<PaymentFailed> PaymentFailed { get; private set; } = null!;

    public Event<TicketIssued> TicketIssued { get; private set; } = null!;
    public Event<TicketFailed> TicketFailed { get; private set; } = null!;

    public Event<BookingFailed> BookingFailed { get; private set; } = null!;

    public State ProcessingPayment { get; private set; } = null!;
    public State ProccessingTicket { get; private set; } = null!;

    public State Completed { get; private set; } = null!;
    public State Failed { get; private set; } = null!;

    public BookingStateMachine()
    {
        Event(() => BookingCreated, x => x.CorrelateById(m => m.Message.BookingId));

        Event(() => PaymentConfirmed, x => x.CorrelateById(m => m.Message.BookingId));
        Event(() => PaymentFailed, x => x.CorrelateById(m => m.Message.BookingId));

        Event(() => TicketIssued, x => x.CorrelateById(m => m.Message.BookingId));
        Event(() => TicketFailed, x => x.CorrelateById(m => m.Message.BookingId));

        Event(() => BookingFailed, x => x.CorrelateById(m => m.Message.BookingId));

        InstanceState(x => x.CurrentState);

        Initially(
            When(BookingCreated)
                .Then(context =>
                {
                    context.Saga.Amount = context.Message.Amount;
                    context.Saga.UserEmail = context.Message.UserEmail;
                    context.Saga.BookingDateUtc = DateTime.UtcNow;
                })
                .PublishAsync(context => context.Init<ProcessPayment>(new ProcessPayment(
                    BookingId: context.Saga.CorrelationId,
                    Amount: context.Saga.Amount,
                    UserId: context.Message.UserId,
                    PaymentIntentId: $"pay_{Guid.NewGuid()}")))
                .TransitionTo(ProcessingPayment)
        );

        During(ProcessingPayment,
            When(PaymentConfirmed)
                .PublishAsync(context => context.Init<IssueTicket>(new IssueTicket(
                    BookingId: context.Saga.CorrelationId,
                    UserEmail: context.Saga.UserEmail))
                    )
                .TransitionTo(ProccessingTicket),
            When(PaymentFailed)
                .TransitionTo(Failed)
                .Finalize()
            );

        During(ProccessingTicket,
            When(TicketIssued)
                .PublishAsync(context => context.Init<BookingConfirmed>(new BookingConfirmed(
                    BookingId: context.Saga.CorrelationId,
                    UserEmail: context.Saga.UserEmail,
                    Amount: context.Saga.Amount))
                    )
                .TransitionTo(Completed),
            When(TicketFailed)
                .TransitionTo(Failed)
                .Finalize()
            );
    }
}

using MassTransit;
using zelavia.Contracts.Messages;

namespace zelavia.BookingsApi.StateMachine;

public class BookingStateMachine : MassTransitStateMachine<BookingState>
{
    // Бронь места (Fligh..Api) -> Снятие денег (PaymentApi) -> Отправка билета (TicketsApi)

    // Booking Created -> ProccessPayment -> ProccessingPayment -> PaymentSucceded -> TicketIssued
    //                                                             PaymentFailed -> BookingFailed
    public Event<BookingCreated> BookingCreated { get; private set; } = null!;
    public Event<PaymentConfirmed> PaymentConfirmed { get; private set; } = null!;
    public Event<PaymentFailed> PaymentFailed { get; private set; } = null!;
    public Event<BookingFailed> BookingFailed { get; private set; } = null!;
    public Event<TicketIssued> TicketIssued { get; private set; } = null!;

    public State ProcessingPayment { get; private set; }
    public State Completed { get; private set; } = null!;
    public State Failed { get; private set; } = null!;
    public BookingStateMachine()
    {
        Event(() => BookingCreated, x => x.CorrelateById(m => m.Message.BookingId));
        Event(() => PaymentConfirmed, x => x.CorrelateById(m => m.Message.BookingId));
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
                    UserEmail: context.Saga.UserEmail)))
                .TransitionTo(ProcessingPayment)
        );

        During(ProcessingPayment,
            When(PaymentConfirmed)
                .PublishAsync(context => context.Init<TicketIssued>(new TicketIssued(
                    BookingId: context.Saga.CorrelationId,
                    UserEmail: context.Saga.UserEmail))
                    )
                .TransitionTo(Completed)
                .Finalize(),
            When(PaymentFailed)
                .PublishAsync(context => context.Init<BookingFailed>(new BookingFailed(
                    BookingId: context.Saga.CorrelationId,
                    UserEmail: context.Saga.UserEmail,
                    Amount: context.Saga.Amount))
                    )
                .TransitionTo(Failed)
                .Finalize()
            );
    }
}


public class BookingState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public State CurrentState { get; set; } = null!;

    public decimal Amount { get; set; }
    public string PaymentIntendId { get; set; } = null!;
    public DateTime BookingDateUtc { get; set; }
    public string UserEmail { get; set; } = null!;
}

namespace zelavia.PaymentsApi.Data;

public class Payment
{
    public Guid BookingId { get; set; }
    public Guid UserId {  get; set; }
    public decimal Amount { get; set; }

    public string PaymentIntentId { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    public Payment(Guid bookingId, Guid userId, decimal amount, string paymentIntentId, DateTime createdOnUtc)
    {
        BookingId = bookingId;
        UserId = userId;
        Amount = amount;
        PaymentIntentId = paymentIntentId;
        CreatedOnUtc  = createdOnUtc;
    }
}

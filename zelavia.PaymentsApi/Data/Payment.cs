namespace zelavia.PaymentsApi.Data;

public class Payment
{
    private Guid BookingId { get; set; }
    private string UserEmail { get; set; }
    private decimal Amount { get; set; }

    public Payment(Guid bookingId, string userEmail, decimal amount)
    {
        BookingId = bookingId;
        UserEmail = userEmail;
        Amount = amount;
    }
}

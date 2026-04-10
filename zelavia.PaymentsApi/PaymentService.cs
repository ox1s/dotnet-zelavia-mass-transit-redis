using zelavia.PaymentsApi.Data;

namespace zelavia.PaymentsApi;

public class PaymentService(HttpClient _httpClient) : IPaymentService
{
    public async Task<Payment?> ProcessPaymentAsync(
        Guid bookingId,
        Guid userId,
        decimal amount,
        string paymentIntentId,
        CancellationToken ct = default)
    {
        var user = await _httpClient.GetFromJsonAsync<UserProfileDto>($"/users/{userId}", ct);

        if (user == null) return null;

        if (user.Wallet < amount)
        {
            throw new ArgumentException("No-no-no");
        }

        var updateResponse = await _httpClient.PostAsJsonAsync($"/users/{userId}/deduct", new { Amount = amount }, ct);
        updateResponse.EnsureSuccessStatusCode();

        return new Payment(bookingId, userId, amount, paymentIntentId, DateTime.UtcNow);
    }
}
public record UserProfileDto(Guid Id, string Email, decimal Wallet);
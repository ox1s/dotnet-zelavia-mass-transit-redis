namespace zelavia.Web;

public class FlightBookingApiClient(HttpClient httpClient)
{
    public async Task<List<Flight>> GetFlightsAsync(CancellationToken cancellationToken = default)
    {
        List<Flight>? flights = null;

        await foreach (var flight in httpClient.GetFromJsonAsAsyncEnumerable<Flight>("/flights", cancellationToken))
        {
            if (flight is not null)
            {
                flights ??= new List<Flight>();
                flights.Add(flight);
            }
        }

        return flights ?? new List<Flight>();
    }

    public async Task BookFlightAsync(Guid userId, string userEmail, Guid flightId, CancellationToken cancellationToken = default)
    {
        await httpClient.PostAsJsonAsync(
            $"/flights/{flightId}/book",
            new
            {
                UserId = userId,
                UserEmail = userEmail,
            },
            cancellationToken);
    }
}
public record Flight(Guid Id, DateTime FlightUtc, decimal Price);

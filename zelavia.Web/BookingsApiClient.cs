namespace zelavia.Web;

public class BookingsApiClient(HttpClient httpClient)
{
    public async Task<List<Arrival>> GetArrivalsAsync(CancellationToken cancellationToken = default)
    {
        List<Arrival>? arrivals = null;

        await foreach (var arrival in httpClient.GetFromJsonAsAsyncEnumerable<Arrival>("/arrivals", cancellationToken))
        {
            if (arrival is not null)
            {
                arrivals ??= [];
                arrivals.Add(arrival);
            }
        }
        return arrivals ?? [];
    }

}
public record Arrival(Guid Id, DateTime ArrivalUtc, decimal Price);

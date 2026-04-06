namespace zelavia.Web;

public class UserApiClient(HttpClient httpClient)
{
    public async Task<List<User>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        List<User>? users = null;

        await foreach (var user in httpClient.GetFromJsonAsAsyncEnumerable<User>("/users", cancellationToken))
        {
            if (user is not null)
            {
                users ??= [];
                users.Add(user);
            }
        }

        return users ?? [];
    }

    public async Task<User?> GetUserAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await httpClient.GetFromJsonAsync<User>($"/users/{id:guid}", cancellationToken);
    }

}
public record User(Guid Id, string Email, decimal Wallet);
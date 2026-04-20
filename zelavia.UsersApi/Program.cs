using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddProblemDetails();

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

List<User> users = [new User(Guid.NewGuid(), "user1@gmail.com", 1000), new User(Guid.NewGuid(), "user2@gmail.com", 100), new User(Guid.NewGuid(), "user3@gmail.com", 10)];

app.MapGet("/users", () =>
{
    return users;
});
app.MapPost("/users", (string email, decimal wallet) =>
{
    var userId = Guid.NewGuid();
    var user = new User(userId, email, wallet);
    users.Add(user);
    return Results.Created($"/users/{userId:guid}/", user);
});

app.MapPost("/users/{id:guid}/deduct", (Guid id, DeductRequest request) =>
{
    var user = users.FirstOrDefault(x => x.Id == id);
    if (user is null) return Results.NotFound();

    user.Wallet -= request.Amount;

    return Results.Ok();
});

app.MapGet("/users/{id:guid}", (Guid id) =>
{
    var user = users.FirstOrDefault(x => x.Id == id);
    return user is null ?
        Results.NotFound()
        : Results.Ok(user);
});


app.MapDefaultEndpoints();

app.Run();

public class User
{
    public User(Guid id, string email, decimal wallet)
    {
        Id = id;
        Email = email;
        Wallet = wallet;
    }

    public Guid Id { get; set; }
    public string Email { get; set; }
    public decimal Wallet { get; set; }


}
public record DeductRequest(decimal Amount);
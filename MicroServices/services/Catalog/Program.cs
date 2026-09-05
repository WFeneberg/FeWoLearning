// A deliberately tiny service. Exercises reference it when they need a REAL
// HTTP resource in the model; it is not itself an exercise and gets no catalog row.
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/health", () => Results.Ok("healthy"));
app.MapGet("/products", () => new[]
{
    new { Id = 1, Name = "Keyboard", Price = 79.90m },
    new { Id = 2, Name = "Monitor", Price = 329.00m }
});

app.Run();

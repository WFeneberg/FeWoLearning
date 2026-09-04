using FeWoLearning.Blazor.Host.Components;
using FeWoLearning.Blazor.Support;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Ex043/Ex045 (CounterStore) and Ex044 (ScopedCounter/SingletonCounter): registered
// here, not per-page, because Program.cs's DI container is shared by the whole host
// app the way a real app's would be - one AddScoped/AddSingleton per type is all a
// demo page needs, unlike a bUnit test's isolated per-test Services collection.
builder.Services.AddScoped<CounterStore>();
builder.Services.AddScoped<ScopedCounter>();
builder.Services.AddSingleton<SingletonCounter>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

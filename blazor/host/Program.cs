using FeWoLearning.Blazor.Exercises.Intermediate;
using FeWoLearning.Blazor.Host.Components;
using FeWoLearning.Blazor.Support;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Ex043/Ex045/Ex046/Ex048 (CounterStore) and Ex044 (ScopedCounter/SingletonCounter):
// registered here, not per-page, because Program.cs's DI container is shared by the
// whole host app the way a real app's would be - one AddScoped/AddSingleton per type
// is all a demo page needs, unlike a bUnit test's isolated per-test Services collection.
builder.Services.AddScoped<CounterStore>();
builder.Services.AddScoped<ScopedCounter>();
builder.Services.AddSingleton<SingletonCounter>();

// Ex047: the options pattern needs an IOptions<GreetingOptions> registered somewhere
// in the container - Configure<T> is how a real app would do it (appsettings-bound in
// practice; a literal value here since this host has no configuration source for it).
builder.Services.Configure<Ex047_OptionsPatternComponent.GreetingOptions>(options => options.Prefix = "Hi there");

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

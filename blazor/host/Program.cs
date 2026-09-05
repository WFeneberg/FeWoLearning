using FeWoLearning.Blazor.Exercises.Advanced;
using FeWoLearning.Blazor.Exercises.Intermediate;
using FeWoLearning.Blazor.Host.Components;
using Microsoft.AspNetCore.Components.Authorization;
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

// Ex067: the custom ErrorBoundary subclass injects this, so it has to exist in the
// host container the same way a real app's logging sink would.
builder.Services.AddScoped<ErrorLog>();

// Ex081/Ex082: AuthorizeView resolves the authorization services out of DI, and the
// exercise's own provider doubles as this host's AuthenticationStateProvider. Note
// what is deliberately NOT here: AddCascadingAuthenticationState(), which would make
// every page in the app read the provider - and in stub mode that provider throws.
// The two demo pages wrap themselves in <CascadingAuthenticationState> instead, so
// only they break while the exercise is unfinished.
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<Ex082_CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(
    sp => sp.GetRequiredService<Ex082_CustomAuthenticationStateProvider>());

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

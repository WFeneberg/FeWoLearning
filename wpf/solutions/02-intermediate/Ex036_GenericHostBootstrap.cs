// Exercise 036 - Generic host bootstrap (intermediate). REFERENCE SOLUTION.
// Goal:   Replace the App.xaml.cs singleton and a bare `new MainViewModel()` sitting in a
//         constructor with a resolved service: build a Microsoft.Extensions.Hosting host,
//         register the shell view model and its dependency in it, and resolve the view
//         model through the container instead of constructing it directly.
// Drills: Host.CreateApplicationBuilder, registering services (AddSingleton), and
//         resolving the shell view model from IServiceProvider instead of `new`ing it.
// Passes: dotnet test --filter FullyQualifiedName~Ex036_

using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FeWoLearning.Wpf.Exercises.Intermediate;

/// <summary>Ready to use - stands in for a real settings/welcome-message service the shell
/// view model would depend on in production code.</summary>
public interface Ex036_IGreeter
{
    string Greet();
}

/// <summary>Ready to use.</summary>
public sealed class Ex036_FixedGreeter(string message) : Ex036_IGreeter
{
    public string Greet() => message;
}

/// <summary>
/// The "shell" view model a real MainWindow would bind its DataContext to. Ready to use -
/// not the subject of this row; what matters is how it gets constructed. See
/// Ex036_GenericHostBootstrap below.
/// </summary>
public sealed class Ex036_ShellViewModel : INotifyPropertyChanged
{
    private string _title;

    public Ex036_ShellViewModel(Ex036_IGreeter greeter)
    {
        _title = greeter.Greet();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Title
    {
        get => _title;
        set
        {
            if (_title == value) return;
            _title = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Title)));
        }
    }
}

public static class Ex036_GenericHostBootstrap
{
    /// <summary>
    /// Builds (does not start) a host whose container can produce an Ex036_ShellViewModel
    /// on demand - registered as a SINGLETON, together with <paramref name="greeter"/> as
    /// the Ex036_IGreeter it depends on. "Registered as a singleton" is an externally
    /// observable contract, not an implementation detail: resolving the view model twice
    /// from the same host must hand back the identical object both times.
    /// </summary>
    public static IHost BuildHost(Ex036_IGreeter greeter)
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Services.AddSingleton(greeter);
        builder.Services.AddSingleton<Ex036_ShellViewModel>();
        return builder.Build();
    }

    /// <summary>
    /// Hands back the shell view model that lives in <paramref name="host"/>'s own
    /// container. This must be a genuine resolution through <paramref name="host"/> -
    /// caching or constructing the view model anywhere outside the container (a static
    /// field, a `new` call) would make this method's result stop being the same object the
    /// container itself would hand back if asked directly, which is exactly what this row
    /// is about.
    /// </summary>
    public static Ex036_ShellViewModel ResolveShellViewModel(IHost host)
        => host.Services.GetRequiredService<Ex036_ShellViewModel>();
}

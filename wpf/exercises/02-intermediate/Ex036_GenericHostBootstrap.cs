// Exercise 036 - Generic host bootstrap (intermediate).
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
    /// Builds (does not start) a host whose container has <paramref name="greeter"/>
    /// registered as the singleton Ex036_IGreeter, and Ex036_ShellViewModel registered as a
    /// singleton, so ResolveShellViewModel below always hands back the same instance for
    /// the life of the host - the seam that replaces a bare `new Ex036_ShellViewModel(...)`.
    /// </summary>
    public static IHost BuildHost(Ex036_IGreeter greeter)
        // TODO: var builder = Host.CreateApplicationBuilder();
        //       builder.Services.AddSingleton(greeter);
        //       builder.Services.AddSingleton<Ex036_ShellViewModel>();
        //       return builder.Build();
        => throw new NotImplementedException("TODO: Ex036 - Host.CreateApplicationBuilder(), register greeter and Ex036_ShellViewModel as singletons, return builder.Build()");

    /// <summary>
    /// Resolves the shell view model from the host's container - never call `new
    /// Ex036_ShellViewModel(...)` here, that would defeat the whole point of the row.
    /// </summary>
    public static Ex036_ShellViewModel ResolveShellViewModel(IHost host)
        // TODO: return host.Services.GetRequiredService<Ex036_ShellViewModel>();
        => throw new NotImplementedException("TODO: Ex036 - return host.Services.GetRequiredService<Ex036_ShellViewModel>()");
}

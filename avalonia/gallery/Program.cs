using Avalonia;
using ReactiveUI.Avalonia;
using ReactiveUI.Builder;

namespace FeWoLearning.Avalonia.Gallery;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        // Same mandatory ReactiveUI initialization the test harness performs.
        RxAppBuilder.CreateReactiveUIBuilder().WithAvalonia().Build();
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}

using ReactiveUI;
using ReactiveUI.Primitives;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

/// Exercise 037 - OutputProperty (intermediate).
/// Goal:   Publish a read-only derived property backed by an
///         ObservableAsPropertyHelper instead of a plain computed getter.
/// Drills: ToProperty, ObservableAsPropertyHelper.
/// Passes: dotnet test --filter FullyQualifiedName~Ex037_
public class Ex037_OutputPropertyViewModel : ReactiveObject
{
    private double _celsius;
    public double Celsius { get => _celsius; set => this.RaiseAndSetIfChanged(ref _celsius, value); }

    private readonly ObservableAsPropertyHelper<double> _fahrenheit;
    public double Fahrenheit => _fahrenheit.Value;

    public Ex037_OutputPropertyViewModel()
    {
        _fahrenheit = this.WhenAnyValue(x => x.Celsius)
            .Select(c => c * 9 / 5 + 32)
            .ToProperty(this, x => x.Fahrenheit);
    }
}

using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Input;
using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Advanced;

// Passes: dotnet test --filter FullyQualifiedName~Ex079_
public class Ex079_KeyBindingsAndAccelerators : StackPanel
{
    /// <summary>Given. Do not change.</summary>
    public List<string> Invoked { get; } = [];

    /// <summary>Given. Do not change.</summary>
    public TextBox Editor { get; } = new() { Width = 120 };

    private void Bind()
    {
        Add(new KeyGesture(Key.S, KeyModifiers.Control), "save");
        Add(new KeyGesture(Key.S, KeyModifiers.Control | KeyModifiers.Shift), "saveAs");
        Add(new KeyGesture(Key.Delete), "delete");

        void Add(KeyGesture gesture, string name) =>
            KeyBindings.Add(new KeyBinding
            {
                Gesture = gesture,
                Command = ReactiveCommand.Create(() => Invoked.Add(name)),
            });
    }

    public static string Describe(string gesture) => KeyGesture.Parse(gesture).ToString();

    public Ex079_KeyBindingsAndAccelerators()
    {
        Children.Add(Editor);
        Bind();
    }
}

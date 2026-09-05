using System.Text.Json;
using FeWoLearning.Security.Exercises.DesktopCore;

namespace FeWoLearning.Security.Tests.DesktopCore;

public class Ex045_UnsafeDeserializationTests
{
    public sealed class Widget
    {
        public string Name { get; set; } = "";
        public int Count { get; set; }
    }

    public sealed class Gadget
    {
        public double Weight { get; set; }
    }

    public sealed class NotAllowed
    {
        public string Payload { get; set; } = "";
    }

    private static readonly Type[] AllowedTypes = [typeof(Widget), typeof(Gadget)];

    private static string Envelope(string typeName, object data)
    {
        var dataJson = JsonSerializer.Serialize(data);
        return $$"""{"type": "{{typeName}}", "data": {{dataJson}}}""";
    }

    [Fact]
    public void Attack_A_Type_Outside_The_Allowlist_Is_Rejected()
    {
        var json = Envelope(typeof(NotAllowed).FullName!, new { Payload = "x" });

        var result = Ex045_UnsafeDeserialization.TryDeserialize(json, AllowedTypes, out var value, out var rejection);

        Assert.False(result);
        Assert.Null(value);
        Assert.NotNull(rejection);
    }

    [Fact]
    public void Attack_An_Allowed_Type_Named_By_Assembly_Qualified_Name_Is_Rejected()
    {
        // Widget is allowed - but not in this qualified form. Accepting it
        // would mean the implementation resolved the string via reflection
        // (e.g. Type.GetType) rather than matching against the allowlist's
        // own type identities directly.
        var json = Envelope(typeof(Widget).AssemblyQualifiedName!, new { Name = "Bob", Count = 1 });

        var result = Ex045_UnsafeDeserialization.TryDeserialize(json, AllowedTypes, out var value, out var rejection);

        Assert.False(result);
        Assert.Null(value);
        Assert.NotNull(rejection);
    }

    [Fact]
    public void Attack_Rejection_Message_Does_Not_Echo_The_Attacker_Supplied_Type_Name()
    {
        const string marker = "TYPE_MARKER_9f21ab6c";
        var json = Envelope($"Attacker.Injected.{marker}", new { });

        var result = Ex045_UnsafeDeserialization.TryDeserialize(json, AllowedTypes, out _, out var rejection);

        Assert.False(result);
        Assert.NotNull(rejection);
        Assert.DoesNotContain(marker, rejection);
    }

    [Fact]
    public void Use_An_Allowed_Type_Deserialises_With_Its_Properties_Populated()
    {
        var json = Envelope(typeof(Widget).FullName!, new { Name = "Bob", Count = 7 });

        var result = Ex045_UnsafeDeserialization.TryDeserialize(json, AllowedTypes, out var value, out var rejection);

        Assert.True(result);
        Assert.Null(rejection);
        var widget = Assert.IsType<Widget>(value);
        Assert.Equal("Bob", widget.Name);
        Assert.Equal(7, widget.Count);
    }

    [Fact]
    public void Use_Two_Different_Allowed_Types_Both_Work_Through_The_Same_Call()
    {
        var widgetJson = Envelope(typeof(Widget).FullName!, new { Name = "Alice", Count = 3 });
        var gadgetJson = Envelope(typeof(Gadget).FullName!, new { Weight = 2.5 });

        Assert.True(Ex045_UnsafeDeserialization.TryDeserialize(widgetJson, AllowedTypes, out var widgetValue, out var widgetRejection));
        Assert.Null(widgetRejection);
        var widget = Assert.IsType<Widget>(widgetValue);
        Assert.Equal("Alice", widget.Name);
        Assert.Equal(3, widget.Count);

        Assert.True(Ex045_UnsafeDeserialization.TryDeserialize(gadgetJson, AllowedTypes, out var gadgetValue, out var gadgetRejection));
        Assert.Null(gadgetRejection);
        var gadget = Assert.IsType<Gadget>(gadgetValue);
        Assert.Equal(2.5, gadget.Weight);
    }
}

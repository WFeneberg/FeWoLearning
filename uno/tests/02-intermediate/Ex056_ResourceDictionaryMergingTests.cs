using FeWoLearning.Uno.Exercises.Intermediate;
using Microsoft.UI.Xaml;

namespace FeWoLearning.Uno.Tests.Intermediate;

public class Ex056_ResourceDictionaryMergingTests : UnoTestContext
{
    private static ResourceDictionary Layer(double value) =>
        Ex056_ResourceDictionaryMerging.CreateLayer("CardWidth", value);

    private static ResourceDictionary Compose(
        IEnumerable<ResourceDictionary> layers,
        params (string Key, double Value)[] own) =>
        Ex056_ResourceDictionaryMerging.Compose(
            layers,
            own.Select(entry => new KeyValuePair<string, double>(entry.Key, entry.Value)));

    [Fact]
    public void A_Layer_Holds_Its_Entry()
    {
        Assert.Equal(40d, Layer(40)["CardWidth"]);
    }

    [Fact]
    public void One_Merged_Layer_Is_Visible()
    {
        var composed = Compose([Layer(40)]);

        Assert.Equal(40, Ex056_ResourceDictionaryMerging.CreateCard(composed).Width, 1);
    }

    [Fact]
    public void The_Last_Merged_Layer_Wins()
    {
        var composed = Compose([Layer(40), Layer(90)]);

        // Later merges shadow earlier ones. Getting the order backwards shows the base
        // value with no error anywhere - which is why a brand layer goes on top.
        Assert.Equal(90, Ex056_ResourceDictionaryMerging.CreateCard(composed).Width, 1);
    }

    [Fact]
    public void An_Own_Key_Outranks_Every_Merged_One()
    {
        var composed = Compose([Layer(40), Layer(90)], ("CardWidth", 15));

        Assert.Equal(15, Ex056_ResourceDictionaryMerging.CreateCard(composed).Width, 1);
    }

    [Fact]
    public void The_Merged_Layers_Are_Kept_In_Order()
    {
        var first = Layer(40);
        var second = Layer(90);

        var composed = Compose([first, second]);

        Assert.Same(first, composed.MergedDictionaries[0]);
        Assert.Same(second, composed.MergedDictionaries[1]);
    }

    [Fact]
    public void A_Missing_Key_Leaves_The_Width_Unset()
    {
        var composed = Compose([Ex056_ResourceDictionaryMerging.CreateLayer("SomethingElse", 40)]);

        var card = Ex056_ResourceDictionaryMerging.CreateCard(composed);

        // Not zero: Width is NaN when nobody requested one (ex034).
        Assert.True(double.IsNaN(card.Width));
    }

    [Fact]
    public void An_Empty_Chain_Leaves_The_Width_Unset()
    {
        var card = Ex056_ResourceDictionaryMerging.CreateCard(Compose([]));

        Assert.True(double.IsNaN(card.Width));
    }

    [Fact]
    public void The_Card_Carries_The_Composed_Dictionary()
    {
        var composed = Compose([Layer(40)]);

        var card = Ex056_ResourceDictionaryMerging.CreateCard(composed);

        Assert.Same(composed, card.Resources);
    }

    [Fact]
    public void A_Merged_Entry_Is_Reachable_Through_The_Composed_Dictionary()
    {
        var composed = Compose([Layer(40)]);

        // The lookup walks the merged chain, so an element with this dictionary sees the
        // entry even though the dictionary itself does not hold the key.
        Assert.True(composed.TryGetValue("CardWidth", out var value));
        Assert.Equal(40d, value);
    }
}

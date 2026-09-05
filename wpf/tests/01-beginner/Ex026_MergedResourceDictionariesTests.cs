using System.Linq;
using System.Windows;
using System.Windows.Controls;
using FeWoLearning.Wpf.Exercises.Beginner;

namespace FeWoLearning.Wpf.Tests.Beginner;

public class Ex026_MergedResourceDictionariesTests : WpfTestContext
{
    [WpfFact]
    public void MergeInOrder_Adds_Both_Dictionaries_In_The_Given_Order()
    {
        var target = new ResourceDictionary();
        var first = new ResourceDictionary { { "A", 1 } };
        var second = new ResourceDictionary { { "B", 2 } };

        Ex026_MergedResourceDictionaries.MergeInOrder(target, first, second);

        Assert.Equal(2, target.MergedDictionaries.Count);
        Assert.Same(first, target.MergedDictionaries[0]);
        Assert.Same(second, target.MergedDictionaries[1]);
    }

    [WpfFact]
    public void On_A_Key_Collision_The_Dictionary_Added_Last_Wins()
    {
        var target = new ResourceDictionary();
        var first = new ResourceDictionary { { "Brush", "FirstBrush" } };
        var second = new ResourceDictionary { { "Brush", "SecondBrush" } };

        Ex026_MergedResourceDictionaries.MergeInOrder(target, first, second);

        // Measured, not assumed: the dictionary added LAST to MergedDictionaries wins a
        // collision - "second" here, even though "first" was merged first.
        Assert.Equal("SecondBrush", target["Brush"]);
    }

    [WpfFact]
    public void Swapping_The_Merge_Order_Swaps_The_Winner()
    {
        // Same two dictionaries, opposite call order - proves the winner tracks add order,
        // not dictionary identity or which parameter is named "first".
        var target = new ResourceDictionary();
        var a = new ResourceDictionary { { "Brush", "ValueA" } };
        var b = new ResourceDictionary { { "Brush", "ValueB" } };

        Ex026_MergedResourceDictionaries.MergeInOrder(target, b, a);

        Assert.Equal("ValueA", target["Brush"]);
    }

    [WpfFact]
    public void AddOwnEntry_Writes_Directly_Into_The_Target_Not_Into_A_Merged_Dictionary()
    {
        var target = new ResourceDictionary();

        Ex026_MergedResourceDictionaries.AddOwnEntry(target, "Greeting", "Hello");

        // target.Keys reflects only the dictionary's OWN entries, never anything reachable
        // only through MergedDictionaries - this is what tells "wrote into target itself"
        // apart from "wrote somewhere the lookup happens to still find".
        Assert.Contains("Greeting", target.Keys.Cast<object>());
        Assert.Equal("Hello", target["Greeting"]);
    }

    [WpfFact]
    public void An_Own_Entry_Always_Wins_Over_Merged_Dictionaries_Regardless_Of_Their_Order()
    {
        var target = new ResourceDictionary();
        var first = new ResourceDictionary { { "Brush", "FromFirst" } };
        var second = new ResourceDictionary { { "Brush", "FromSecond" } };
        Ex026_MergedResourceDictionaries.MergeInOrder(target, first, second);

        Ex026_MergedResourceDictionaries.AddOwnEntry(target, "Brush", "FromTargetItself");

        Assert.Equal("FromTargetItself", target["Brush"]);
    }

    [WpfFact]
    public void A_Live_Elements_FindResource_Follows_The_Same_Last_Wins_Order()
    {
        var root = new StackPanel();
        var first = new ResourceDictionary { { "Brush", "FromFirst" } };
        var second = new ResourceDictionary { { "Brush", "FromSecond" } };
        Ex026_MergedResourceDictionaries.MergeInOrder(root.Resources, first, second);
        var child = new Button();
        root.Children.Add(child);
        Layout(root);

        Assert.Equal("FromSecond", child.FindResource("Brush"));
    }
}

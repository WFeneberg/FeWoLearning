using System.Globalization;
using System.Text.RegularExpressions;
using AngleSharp.Dom;
using Bunit;
using FeWoLearning.Blazor.Exercises.Advanced;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Advanced;

public class Ex074_VirtualizeFixedItemSizeTests : BunitContext
{
    private const int Total = 500;

    /// <Virtualize> brackets its realised window with two spacer divs whose height
    /// stands in for the rows that are not in the DOM. The trailing one is where
    /// ItemSize becomes visible without a browser.
    private static double TrailingSpacerHeight(IRenderedComponent<Ex074_VirtualizeFixedItemSize> cut)
    {
        var style = cut.FindAll("div")
            .Select(div => div.GetAttribute("style"))
            .Last(style => style is not null)!;

        var height = Regex.Match(style, @"height:\s*([0-9.]+)px").Groups[1].Value;
        return double.Parse(height, CultureInfo.InvariantCulture);
    }

    private static ValueTask<ItemsProviderResult<string>> FullPage(ItemsProviderRequest request)
    {
        var count = Math.Min(request.Count, Total - request.StartIndex);
        return ValueTask.FromResult(new ItemsProviderResult<string>(
            Enumerable.Range(request.StartIndex, count).Select(i => $"item-{i}"),
            Total));
    }

    // A provider that answers with fewer items than it was asked for leaves the rest
    // of the window unfilled - which is the only way a windowless test can see a
    // Placeholder at all, and is exactly the case the fragment exists for.
    [Fact]
    public void Slots_The_Provider_Did_Not_Fill_Show_The_Placeholder()
    {
        ItemsProviderRequest? seen = null;

        var cut = Render<Ex074_VirtualizeFixedItemSize>(p => p
            .Add(c => c.Provider, request =>
            {
                seen = request;
                return ValueTask.FromResult(new ItemsProviderResult<string>(
                    Enumerable.Range(request.StartIndex, 10).Select(i => $"item-{i}"),
                    Total));
            })
            .Add(c => c.OverscanCount, 0));

        var rows = cut.FindAll(".row").Count;
        var placeholders = cut.FindAll(".placeholder").Count;

        Assert.Equal(10, rows);
        Assert.True(placeholders > 0, "the unfilled part of the window should be placeholders");
        // Window fully accounted for, without hardcoding how big it is.
        Assert.Equal(seen!.Value.Count, rows + placeholders);
    }

    [Fact]
    public void A_Fully_Answered_Window_Has_No_Placeholders()
    {
        var cut = Render<Ex074_VirtualizeFixedItemSize>(p => p.Add(c => c.Provider, FullPage));

        Assert.NotEmpty(cut.FindAll(".row"));
        Assert.Empty(cut.FindAll(".placeholder"));
    }

    // ItemSize does not decide how many rows are realised - a real browser's viewport
    // does, and there is none here. What it decides is how much space the unrealised
    // rows are reserved, which is the invariant asserted here.
    [Fact]
    public void ItemSize_Sizes_The_Space_Held_For_The_Rows_That_Are_Not_There()
    {
        var cut = Render<Ex074_VirtualizeFixedItemSize>(p => p
            .Add(c => c.Provider, FullPage)
            .Add(c => c.ItemSize, 20f));

        var rendered = cut.FindAll(".row").Count;
        Assert.Equal((Total - rendered) * 20d, TrailingSpacerHeight(cut));

        var bigger = Render<Ex074_VirtualizeFixedItemSize>(p => p
            .Add(c => c.Provider, FullPage)
            .Add(c => c.ItemSize, 80f));

        Assert.Equal(rendered, bigger.FindAll(".row").Count);
        Assert.Equal((Total - rendered) * 80d, TrailingSpacerHeight(bigger));
    }

    // Overscan is the knob that does move the window: it realises rows beyond what is
    // strictly needed, so scrolling has something ready. Measured here as more rows
    // for the same everything else (100 -> 120 for an overscan of 10, at the time of
    // writing); the assertion is the direction, not bUnit's window arithmetic.
    [Fact]
    public void OverscanCount_Widens_The_Realised_Window()
    {
        var tight = Render<Ex074_VirtualizeFixedItemSize>(p => p
            .Add(c => c.Provider, FullPage)
            .Add(c => c.OverscanCount, 0));

        var loose = Render<Ex074_VirtualizeFixedItemSize>(p => p
            .Add(c => c.Provider, FullPage)
            .Add(c => c.OverscanCount, 10));

        Assert.True(
            loose.FindAll(".row").Count > tight.FindAll(".row").Count,
            "a larger OverscanCount should realise more rows");
    }
}

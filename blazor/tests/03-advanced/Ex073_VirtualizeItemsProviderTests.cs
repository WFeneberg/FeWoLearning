using Bunit;
using FeWoLearning.Blazor.Exercises.Advanced;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Advanced;

// There is no browser here, so <Virtualize> never learns a real viewport size and
// falls back to a fixed window (README §7). Nothing below depends on how big that
// window is - only on it being a window rather than the whole list.
public class Ex073_VirtualizeItemsProviderTests : BunitContext
{
    private const int Total = 500;

    private readonly List<ItemsProviderRequest> _requests = [];

    private ValueTask<ItemsProviderResult<string>> Page(ItemsProviderRequest request)
    {
        _requests.Add(request);
        var count = Math.Min(request.Count, Total - request.StartIndex);
        return ValueTask.FromResult(new ItemsProviderResult<string>(
            Enumerable.Range(request.StartIndex, count).Select(i => $"item-{i}"),
            Total));
    }

    [Fact]
    public void Asks_The_Provider_For_A_Window_Rather_Than_The_Whole_List()
    {
        Render<Ex073_VirtualizeItemsProvider>(p => p.Add(c => c.Provider, Page));

        var request = Assert.Single(_requests);
        Assert.Equal(0, request.StartIndex);
        Assert.InRange(request.Count, 1, Total - 1);
    }

    [Fact]
    public void Renders_The_Items_The_Provider_Returned_In_Order()
    {
        var cut = Render<Ex073_VirtualizeItemsProvider>(p => p.Add(c => c.Provider, Page));

        var rows = cut.FindAll(".row");
        Assert.Equal("item-0", rows[0].TextContent);
        Assert.Equal("item-1", rows[1].TextContent);
        Assert.Equal("item-2", rows[2].TextContent);
    }

    // The point of virtualizing: 500 items exist, far fewer are in the DOM.
    [Fact]
    public void Realises_Far_Fewer_Rows_Than_There_Are_Items()
    {
        var cut = Render<Ex073_VirtualizeItemsProvider>(p => p.Add(c => c.Provider, Page));

        var rendered = cut.FindAll(".row").Count;
        Assert.InRange(rendered, 1, Total - 1);
        Assert.Equal(_requests.Single().Count, rendered);
    }

    // Non-vacuity for ItemsProviderResult's second argument: the component sizes
    // itself by the total the result declares, not by how many items came back and
    // not by how many were asked for. Here the provider hands over three items and
    // says three, so three rows is all there is - no placeholders for a phantom tail.
    [Fact]
    public void The_Total_Comes_From_The_Result_Not_From_The_Request()
    {
        var cut = Render<Ex073_VirtualizeItemsProvider>(p => p.Add(
            c => c.Provider,
            _ => ValueTask.FromResult(new ItemsProviderResult<string>(["a", "b", "c"], 3))));

        Assert.Equal(3, cut.FindAll(".row").Count);
    }
}

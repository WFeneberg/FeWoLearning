using System.Collections.Generic;
using System.Threading.Tasks;
using FeWoLearning.Exercises.Intermediate;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Intermediate;

public class Ex046_AsyncDataFetcherTests
{
    [Fact]
    public async Task FetchDataAsync_ReturnsFormattedResult()
    {
        var result = await AsyncDataFetcher.FetchDataAsync("alpha", delayMilliseconds: 0);

        Assert.Equal("Data:alpha", result);
    }

    [Fact]
    public async Task FetchDataAsync_DifferentKey_ReturnsDifferentResult()
    {
        var result = await AsyncDataFetcher.FetchDataAsync("beta", delayMilliseconds: 0);

        Assert.Equal("Data:beta", result);
    }

    [Fact]
    public async Task FetchAllAsync_PreservesInputOrder()
    {
        var keys = new[] { "one", "two", "three" };

        IReadOnlyList<string> results = await AsyncDataFetcher.FetchAllAsync(keys, delayMilliseconds: 0);

        Assert.Equal(new[] { "Data:one", "Data:two", "Data:three" }, results);
    }

    [Fact]
    public async Task FetchAllAsync_EmptyInput_ReturnsEmptyResult()
    {
        var results = await AsyncDataFetcher.FetchAllAsync(System.Array.Empty<string>(), delayMilliseconds: 0);

        Assert.Empty(results);
    }
}

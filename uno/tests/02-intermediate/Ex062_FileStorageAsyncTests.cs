using FeWoLearning.Uno.Exercises.Intermediate;
using Windows.Storage;

namespace FeWoLearning.Uno.Tests.Intermediate;

public class Ex062_FileStorageAsyncTests : UnoTestContext
{
    /// <summary>A fresh subfolder of the temporary folder per test.</summary>
    private static async Task<Ex062_FileStorageAsync> StoreAsync()
    {
        var folder = await ApplicationData.Current.TemporaryFolder.CreateFolderAsync(
            "Ex062_" + Guid.NewGuid().ToString("N"),
            CreationCollisionOption.FailIfExists);

        return new Ex062_FileStorageAsync(folder);
    }

    [Fact]
    public async Task A_Saved_Document_Comes_Back()
    {
        var store = await StoreAsync();

        await store.SaveAsync("notes.txt", "hello");

        Assert.Equal("hello", await store.LoadAsync("notes.txt"));
    }

    [Fact]
    public async Task A_Missing_Document_Reads_As_Null()
    {
        var store = await StoreAsync();

        // Not an exception: GetFileAsync would throw FileNotFoundException here, and a
        // read path that tolerates absence should not be built out of a catch block.
        Assert.Null(await store.LoadAsync("nothing.txt"));
    }

    [Fact]
    public async Task Saving_Twice_Replaces_The_Content()
    {
        var store = await StoreAsync();

        await store.SaveAsync("notes.txt", "a much longer first version");
        await store.SaveAsync("notes.txt", "short");

        // OpenIfExists plus a write leaves the tail of the previous document behind - a
        // corruption that only shows up when the second version is shorter.
        Assert.Equal("short", await store.LoadAsync("notes.txt"));
    }

    [Fact]
    public async Task Deleting_Reports_Success()
    {
        var store = await StoreAsync();
        await store.SaveAsync("notes.txt", "hello");

        Assert.True(await store.DeleteAsync("notes.txt"));
        Assert.Null(await store.LoadAsync("notes.txt"));
    }

    [Fact]
    public async Task Deleting_Something_Absent_Reports_Failure()
    {
        var store = await StoreAsync();

        Assert.False(await store.DeleteAsync("nothing.txt"));
    }

    [Fact]
    public async Task An_Empty_Store_Lists_Nothing()
    {
        var store = await StoreAsync();

        Assert.Empty(await store.ListAsync());
    }

    [Fact]
    public async Task The_Listing_Names_Every_Document()
    {
        var store = await StoreAsync();

        await store.SaveAsync("a.txt", "1");
        await store.SaveAsync("b.txt", "2");

        Assert.Equal(["a.txt", "b.txt"], (await store.ListAsync()).Order());
    }

    [Fact]
    public async Task An_Empty_Document_Is_Not_A_Missing_One()
    {
        var store = await StoreAsync();

        await store.SaveAsync("empty.txt", "");

        Assert.Equal("", await store.LoadAsync("empty.txt"));
    }

    [Fact]
    public async Task Two_Stores_Do_Not_See_Each_Other()
    {
        var first = await StoreAsync();
        var second = await StoreAsync();

        await first.SaveAsync("notes.txt", "hello");

        Assert.Null(await second.LoadAsync("notes.txt"));
    }
}

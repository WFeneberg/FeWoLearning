using FeWoLearning.Architecture.Exercises.Desktop.Ex022;
using FeWoLearning.Architecture.Tests.Harness;

namespace FeWoLearning.Architecture.Tests.Desktop;

public class Ex022_OfflineFirstSyncTests : IDisposable
{
    private readonly SqliteScratch _scratch = new();
    private readonly LocalStore _local;

    public Ex022_OfflineFirstSyncTests()
    {
        _local = new LocalStore(_scratch.ConnectionString);
        _local.EnsureCreated();
    }

    public void Dispose() => _scratch.Dispose();

    /// <summary>A second store over the same file - what a restart of the app looks like.</summary>
    private LocalStore AfterRestart() => new(_scratch.ConnectionString);

    [Fact]
    public void A_Note_Only_The_Server_Has_Is_Pulled_And_Persisted()
    {
        var result = Ex022_OfflineFirstSync.Sync(_local, [new ServerNote("n1", "from server", 4)]);

        Assert.Equal(1, result.Pulled);

        // Read it back through a fresh store: an implementation that reconciled in
        // memory and forgot to write through passes an in-memory assertion and loses
        // everything the moment the application closes.
        var persisted = AfterRestart().Find("n1");
        Assert.NotNull(persisted);
        Assert.Equal("from server", persisted.Text);
        Assert.Equal(4, persisted.BaseVersion);
        Assert.False(persisted.IsDirty);
    }

    [Fact]
    public void A_Clean_Local_Note_Behind_The_Server_Is_Overwritten()
    {
        _local.Upsert(new LocalNote("n1", "old", 1, IsDirty: false));

        var result = Ex022_OfflineFirstSync.Sync(_local, [new ServerNote("n1", "newer", 2)]);

        Assert.Equal(1, result.Pulled);
        Assert.Equal("newer", _local.Find("n1")!.Text);
    }

    [Fact]
    public void Adversarial_A_Clean_Local_Note_Already_Current_Is_Left_Alone()
    {
        // Overwriting unconditionally produces the same text and reports a pull that
        // never happened - which is how a sync UI ends up claiming to have downloaded
        // ten thousand unchanged notes.
        _local.Upsert(new LocalNote("n1", "same", 2, IsDirty: false));

        var result = Ex022_OfflineFirstSync.Sync(_local, [new ServerNote("n1", "same", 2)]);

        Assert.Equal(0, result.Pulled);
        Assert.Equal(0, result.Pushed);
    }

    [Fact]
    public void A_Dirty_Note_The_Server_Has_Not_Moved_Is_Pushed()
    {
        _local.Upsert(new LocalNote("n1", "my edit", 2, IsDirty: true));

        var result = Ex022_OfflineFirstSync.Sync(_local, [new ServerNote("n1", "server copy", 2)]);

        Assert.Equal(1, result.Pushed);
        Assert.Empty(result.Conflicts);

        var after = _local.Find("n1")!;
        Assert.Equal("my edit", after.Text);
        Assert.Equal(3, after.BaseVersion);
        Assert.False(after.IsDirty);
    }

    [Fact]
    public void Mechanism_A_Dirty_Note_Under_A_Moved_Server_Is_Reported_Not_Silently_Dropped()
    {
        // The fact this exercise exists for. "Last write wins" is a perfectly ordinary
        // implementation: it passes every count above, leaves exactly this final state,
        // and destroys work somebody did on a train with no signal. What separates the
        // two is whether the losing text comes back out.
        _local.Upsert(new LocalNote("n1", "my offline edit", 2, IsDirty: true));

        var result = Ex022_OfflineFirstSync.Sync(_local, [new ServerNote("n1", "somebody elses edit", 5)]);

        var conflict = Assert.Single(result.Conflicts);
        Assert.Equal("n1", conflict.Id);
        Assert.Equal("my offline edit", conflict.LocalText);
        Assert.Equal("somebody elses edit", conflict.ServerText);

        // The server still wins locally - the policy is "server wins, loudly".
        Assert.Equal("somebody elses edit", _local.Find("n1")!.Text);
        Assert.False(_local.Find("n1")!.IsDirty);
    }

    [Fact]
    public void A_Dirty_Note_The_Server_Has_Never_Seen_Is_Pushed_As_New()
    {
        _local.Upsert(new LocalNote("local-only", "written offline", 0, IsDirty: true));

        var result = Ex022_OfflineFirstSync.Sync(_local, []);

        Assert.Equal(1, result.Pushed);
        Assert.False(AfterRestart().Find("local-only")!.IsDirty);
    }
}

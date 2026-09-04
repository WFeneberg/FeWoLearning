// Exercise 062 - File Storage Async (intermediate).
// Goal:   Write a file, read it back, and handle the one that is not there.
// Drills: StorageFolder/StorageFile, CreationCollisionOption, FileIO, and
//         GetFileAsync throwing rather than returning null for a missing file.
// Passes: dotnet test --filter FullyQualifiedName~Ex062_
//
// The WinRT storage API is asynchronous all the way down, and it signals "not found" with
// a FileNotFoundException rather than a null - so a read path that has to tolerate a
// missing file needs TryGetItemAsync, not a try/catch around the happy path.

using Windows.Storage;

namespace FeWoLearning.Uno.Exercises.Intermediate;

/// <summary>
/// A tiny document store in a folder: write by name, read by name, delete by name.
/// </summary>
public sealed class Ex062_FileStorageAsync
{
    private readonly StorageFolder _folder;

    public Ex062_FileStorageAsync(StorageFolder folder) => _folder = folder;

    /// <summary>
    /// Writes <paramref name="content"/> to <paramref name="name"/>, replacing whatever was
    /// there.
    /// </summary>
    public async Task SaveAsync(string name, string content)
    {
        // ReplaceExisting, not OpenIfExists: opening and writing leaves the tail of a
        // longer previous document behind, a corruption that only appears when the new
        // version is shorter than the old one.
        var file = await _folder.CreateFileAsync(name, CreationCollisionOption.ReplaceExisting);

        await FileIO.WriteTextAsync(file, content);
    }

    /// <summary>
    /// Reads <paramref name="name"/>, or returns null when there is no such file.
    /// </summary>
    public async Task<string?> LoadAsync(string name) =>
        // TryGetItemAsync answers null; GetFileAsync throws FileNotFoundException. A read
        // path that tolerates absence should not be built out of a catch block.
        await _folder.TryGetItemAsync(name) is StorageFile file
            ? await FileIO.ReadTextAsync(file)
            : null;

    /// <summary>
    /// Deletes <paramref name="name"/> and reports whether there was anything to delete.
    /// </summary>
    public async Task<bool> DeleteAsync(string name)
    {
        if (await _folder.TryGetItemAsync(name) is not StorageFile file)
        {
            return false;
        }

        await file.DeleteAsync();
        return true;
    }

    /// <summary>The names currently in the folder, in whatever order it reports them.</summary>
    public async Task<IReadOnlyList<string>> ListAsync() =>
        (await _folder.GetFilesAsync()).Select(file => file.Name).ToList();
}

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
    public async Task SaveAsync(string name, string content) =>
        // TODO: create the file with CreationCollisionOption.ReplaceExisting and write the
        // text. OpenIfExists would leave the tail of a longer previous document behind.
        throw new NotImplementedException("TODO: Ex062 - save the document");

    /// <summary>
    /// Reads <paramref name="name"/>, or returns null when there is no such file.
    /// </summary>
    public async Task<string?> LoadAsync(string name) =>
        // TODO: use TryGetItemAsync so a missing file is a null rather than an exception,
        // then read the text off it.
        throw new NotImplementedException("TODO: Ex062 - load the document if it exists");

    /// <summary>
    /// Deletes <paramref name="name"/> and reports whether there was anything to delete.
    /// </summary>
    public async Task<bool> DeleteAsync(string name) =>
        throw new NotImplementedException("TODO: Ex062 - delete the document if it exists");

    /// <summary>The names currently in the folder, in whatever order it reports them.</summary>
    public async Task<IReadOnlyList<string>> ListAsync() =>
        throw new NotImplementedException("TODO: Ex062 - list the documents");
}

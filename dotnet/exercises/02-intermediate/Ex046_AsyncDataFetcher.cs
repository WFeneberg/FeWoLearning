using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeWoLearning.Exercises.Intermediate;

// Exercise 046 — AsyncDataFetcher (intermediate).
// Goal:   Implement two async methods that simulate fetching remote data:
//         FetchDataAsync awaits a simulated network delay (via Task.Delay)
//         before returning a formatted result for a single key, and
//         FetchAllAsync fans out FetchDataAsync over a collection of keys
//         concurrently (Task.WhenAll) while preserving input order.
// Drills: async/await, Task<T> return types, Task.Delay, Task.WhenAll,
//         preserving order across concurrent operations, argument validation.
public static class AsyncDataFetcher
{
    // Simulates an asynchronous fetch of data for the given key. The delay
    // parameter models network/IO latency; callers may pass 0 for fast,
    // deterministic execution (e.g. in tests).
    public static Task<string> FetchDataAsync(string key, int delayMilliseconds = 0)
        => throw new NotImplementedException();

    // Fetches data for every key in the sequence concurrently and returns the
    // results in the same order as the input keys (not completion order).
    public static Task<IReadOnlyList<string>> FetchAllAsync(IEnumerable<string> keys, int delayMilliseconds = 0)
        => throw new NotImplementedException();
}

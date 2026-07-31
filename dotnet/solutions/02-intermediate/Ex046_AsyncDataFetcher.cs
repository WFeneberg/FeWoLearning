using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeWoLearning.Exercises.Intermediate;

// Exercise 046 — AsyncDataFetcher (reference solution).
public static class AsyncDataFetcher
{
    public static async Task<string> FetchDataAsync(string key, int delayMilliseconds = 0)
    {
        if (key is null) throw new ArgumentNullException(nameof(key));

        await Task.Delay(delayMilliseconds).ConfigureAwait(false);

        return $"Data:{key}";
    }

    public static async Task<IReadOnlyList<string>> FetchAllAsync(IEnumerable<string> keys, int delayMilliseconds = 0)
    {
        if (keys is null) throw new ArgumentNullException(nameof(keys));

        var tasks = keys.Select(key => FetchDataAsync(key, delayMilliseconds)).ToList();
        var results = await Task.WhenAll(tasks).ConfigureAwait(false);

        return results;
    }
}

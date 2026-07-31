namespace FeWoLearning.Exercises.Intermediate;

// Exercise 069 — Custom Hash Set (reference solution).
public class CustomHashSet
{
    private readonly List<string>[] _buckets;

    public CustomHashSet(int bucketCount = 16)
    {
        if (bucketCount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(bucketCount));
        }

        _buckets = new List<string>[bucketCount];
        for (var i = 0; i < bucketCount; i++)
        {
            _buckets[i] = new List<string>();
        }
    }

    public int Count { get; private set; }

    public bool Add(string item)
    {
        var bucket = GetBucket(item);
        if (bucket.Contains(item))
        {
            return false;
        }

        bucket.Add(item);
        Count++;
        return true;
    }

    public bool Contains(string item) => GetBucket(item).Contains(item);

    private List<string> GetBucket(string item)
    {
        var index = (item.GetHashCode() & 0x7FFFFFFF) % _buckets.Length;
        return _buckets[index];
    }
}

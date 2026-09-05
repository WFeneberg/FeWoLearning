namespace FeWoLearning.Security.Exercises.DesktopWpf;

// Exercise 054 — SensitiveBufferLifetime (reference solution).
public static class Ex054_SensitiveBufferLifetime
{
    public static T UseThenClear<T>(char[] secret, Func<char[], T> work)
    {
        ArgumentNullException.ThrowIfNull(secret);
        ArgumentNullException.ThrowIfNull(work);

        try
        {
            return work(secret);
        }
        finally
        {
            Array.Clear(secret);
        }
    }
}

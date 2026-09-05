namespace FeWoLearning.Security.Exercises.DesktopWpf;

// Exercise 054 — SensitiveBufferLifetime (desktop-wpf).
// Goal:   A `char[]` holding a secret (a password typed into a PasswordBox via
//         SecurePassword, a decrypted token, ...) must not keep living in managed
//         memory once whatever needed it is done - a heap dump or a swapped page
//         taken moments later must not still show the plaintext. UseThenClear runs
//         `work` against the caller's own array and guarantees the array is wiped
//         before returning, no matter how `work` behaves.
// Drills: clearing sensitive buffers, bounded lifetime of plaintext.
// Passes: attack facts   - after UseThenClear returns, every element of the array
//                          the caller passed in is '\0'; the array is cleared even
//                          when `work` throws (the caller catches the exception,
//                          then inspects the array).
//         use facts      - UseThenClear returns exactly the value `work` produced,
//                          unchanged; and `work` is invoked with the array still
//                          holding its ORIGINAL characters, not zeros - the fact
//                          that rules out an implementation that clears the array
//                          before ever calling `work`.
public static class Ex054_SensitiveBufferLifetime
{
    public static T UseThenClear<T>(char[] secret, Func<char[], T> work) =>
        throw new NotImplementedException(
            "TODO: Ex054 - call work(secret) and return its result, but zero every element of secret " +
            "before returning - even when work throws");
}

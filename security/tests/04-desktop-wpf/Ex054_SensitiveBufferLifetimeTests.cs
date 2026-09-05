using FeWoLearning.Security.Exercises.DesktopWpf;

namespace FeWoLearning.Security.Tests.DesktopWpf;

public class Ex054_SensitiveBufferLifetimeTests
{
    [WpfFact]
    public void Attack_Every_Element_Is_Zeroed_After_The_Call_Returns()
    {
        var secret = "hunter2".ToCharArray();

        Ex054_SensitiveBufferLifetime.UseThenClear(secret, chars => chars.Length);

        Assert.All(secret, c => Assert.Equal('\0', c));
    }

    [WpfFact]
    public void Attack_The_Array_Is_Cleared_Even_When_Work_Throws()
    {
        var secret = "hunter2".ToCharArray();

        Assert.Throws<InvalidOperationException>(() =>
            Ex054_SensitiveBufferLifetime.UseThenClear<int>(secret, _ => throw new InvalidOperationException("boom")));

        Assert.All(secret, c => Assert.Equal('\0', c));
    }

    [WpfFact]
    public void Use_The_Value_Work_Returns_Is_Passed_Through_Unchanged()
    {
        var secret = "hunter2".ToCharArray();

        var result = Ex054_SensitiveBufferLifetime.UseThenClear(secret, chars => chars.Length * 2);

        Assert.Equal(14, result);
    }

    [WpfFact]
    public void Use_Work_Observes_The_Original_Characters_Not_Zeros()
    {
        var secret = "hunter2".ToCharArray();
        char[]? observed = null;

        Ex054_SensitiveBufferLifetime.UseThenClear<object?>(secret, chars =>
        {
            observed = (char[])chars.Clone();
            return null;
        });

        Assert.Equal("hunter2".ToCharArray(), observed);
    }
}

using FeWoLearning.Architecture.Exercises.Runtime.Ex096;

namespace FeWoLearning.Architecture.Tests.Runtime;

public class Ex096_GracefulDegradationTests
{
    private sealed class Counter
    {
        public int Calls { get; private set; }

        public string Working(string value) { Calls++; return value; }

        public string Broken() { Calls++; throw new InvalidOperationException("upstream is down"); }
    }

    private static Source Working(string name, Quality quality, Counter counter, string value) =>
        new(name, quality, () => counter.Working(value));

    private static Source Broken(string name, Quality quality, Counter counter) =>
        new(name, quality, counter.Broken);

    [Fact]
    public void Mechanism_The_First_Working_Source_Answers_And_The_Rest_Are_Not_Tried()
    {
        // A chain that tries everything and picks the best is a different design with a
        // different cost - it pays for every source on every request.
        var best = new Counter();
        var fallback = new Counter();

        var answer = Ex096_GracefulDegradation.Resolve(
        [
            Working("personalised", Quality.Full, best, "<for you>"),
            Working("generic", Quality.Degraded, fallback, "<popular>"),
        ]);

        Assert.Equal("<for you>", answer.Value);
        Assert.Equal(1, best.Calls);
        Assert.Equal(0, fallback.Calls);
    }

    [Fact]
    public void A_Failing_Source_Falls_Through_To_The_Next()
    {
        var broken = new Counter();
        var fallback = new Counter();

        var answer = Ex096_GracefulDegradation.Resolve(
        [
            Broken("personalised", Quality.Full, broken),
            Working("generic", Quality.Degraded, fallback, "<popular>"),
        ]);

        Assert.Equal("<popular>", answer.Value);
        Assert.Equal(1, fallback.Calls);
    }

    [Fact]
    public void Mechanism_The_Answer_Carries_Its_Quality_And_Its_Source()
    {
        // The dangerous thing about a fallback chain is that it WORKS. Personalised
        // recommendations fail, the generic ones go out, conversion drops four percent,
        // and nothing anywhere is red - because from the outside the system is serving
        // 200s with plausible content. Six weeks later somebody notices the revenue.
        var answer = Ex096_GracefulDegradation.Resolve(
        [
            Broken("personalised", Quality.Full, new Counter()),
            Working("generic", Quality.Degraded, new Counter(), "<popular>"),
        ]);

        Assert.Equal(Quality.Degraded, answer.Quality);
        Assert.Equal("generic", answer.Source);
    }

    [Fact]
    public void Adversarial_Every_Source_Tried_Is_Reported_Including_The_Failures()
    {
        // "We degraded" is only actionable with "and here is what broke". Recording only
        // the source that succeeded loses the one piece of information somebody needs.
        var answer = Ex096_GracefulDegradation.Resolve(
        [
            Broken("personalised", Quality.Full, new Counter()),
            Broken("collaborative", Quality.Degraded, new Counter()),
            Working("bestsellers", Quality.Minimal, new Counter(), "<top 10>"),
        ]);

        Assert.Equal(["personalised", "collaborative", "bestsellers"], answer.Tried);
        Assert.Equal(Quality.Minimal, answer.Quality);
    }

    [Fact]
    public void Mechanism_When_Everything_Fails_The_Failure_Is_Loud_And_Names_Them_All()
    {
        // A silent empty result is indistinguishable from "there is nothing", which is a
        // different answer with different consequences - an empty basket page is a bug,
        // an empty recommendations panel is Tuesday.
        var failure = Assert.Throws<NoFallbackLeftException>(() => Ex096_GracefulDegradation.Resolve(
        [
            Broken("personalised", Quality.Full, new Counter()),
            Broken("generic", Quality.Degraded, new Counter()),
        ]));

        Assert.Equal(["personalised", "generic"], failure.Tried);
    }

    [Fact]
    public void The_Full_Quality_Answer_Reports_Full()
    {
        // Pairs with the degradation fact: an implementation that always reports Degraded
        // is as useless as one that always reports Full, and much more annoying.
        var answer = Ex096_GracefulDegradation.Resolve(
            [Working("personalised", Quality.Full, new Counter(), "<for you>")]);

        Assert.Equal(Quality.Full, answer.Quality);
        Assert.Equal(["personalised"], answer.Tried);
    }

    [Fact]
    public void An_Empty_Chain_Fails_Rather_Than_Returning_Nothing()
    {
        var failure = Assert.Throws<NoFallbackLeftException>(() => Ex096_GracefulDegradation.Resolve([]));

        Assert.Empty(failure.Tried);
    }
}

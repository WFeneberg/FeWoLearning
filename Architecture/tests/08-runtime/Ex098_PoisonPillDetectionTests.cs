using FeWoLearning.Architecture.Exercises.Runtime.Ex098;

namespace FeWoLearning.Architecture.Tests.Runtime;

public class Ex098_PoisonPillDetectionTests
{
    private static FailureClassifier Classifier() => new(transientAttemptsBeforePoison: 3);

    [Theory]
    [InlineData("JsonException")]
    [InlineData("FormatException")]
    [InlineData("ValidationException")]
    public void Mechanism_A_Shape_Failure_Is_Poison_On_The_First_Attempt(string exceptionType)
    {
        // The payload will not improve. Retrying it three times just delays the same answer
        // while holding a consumer slot - and the message ends up dead-lettered anyway,
        // minutes later.
        var verdict = Classifier().Classify(new Attempt("m-1", exceptionType, AttemptNumber: 1));

        Assert.Equal(Verdict.Poison, verdict);
    }

    [Theory]
    [InlineData("TimeoutException")]
    [InlineData("SocketException")]
    public void An_Infrastructure_Failure_Starts_Out_Transient(string exceptionType)
    {
        // These recover. Dead-lettering one takes a perfectly good message out of the
        // system because a database was restarting.
        var verdict = Classifier().Classify(new Attempt("m-1", exceptionType, AttemptNumber: 1));

        Assert.Equal(Verdict.Transient, verdict);
    }

    [Fact]
    public void Mechanism_A_Repeatedly_Failing_Message_Is_Poison_When_Others_Are_Fine()
    {
        // Failing alone is the evidence. Its neighbours are getting through, so whatever
        // is wrong travelled with this message.
        var classifier = Classifier();
        classifier.RecordOutcome("m-2", succeeded: true);
        classifier.RecordOutcome("m-3", succeeded: true);

        var verdict = classifier.Classify(new Attempt("m-1", "TimeoutException", AttemptNumber: 3));

        Assert.Equal(Verdict.Poison, verdict);
    }

    [Fact]
    public void Mechanism_The_Same_Message_Stays_Transient_While_Everything_Else_Fails_Too()
    {
        // The fact this exercise exists for. A queue where everything times out is a broken
        // dependency, and dead-lettering the whole queue turns a ten-minute outage into a
        // day of manual replay. Attempt count alone cannot tell the difference: the
        // exception type is identical either way.
        var classifier = Classifier();
        classifier.RecordOutcome("m-2", succeeded: false);
        classifier.RecordOutcome("m-3", succeeded: false);

        var verdict = classifier.Classify(new Attempt("m-1", "TimeoutException", AttemptNumber: 3));

        Assert.Equal(Verdict.Transient, verdict);
    }

    [Fact]
    public void Adversarial_The_Messages_Own_Earlier_Successes_Are_Not_Evidence_About_Itself()
    {
        // A classifier that looks at "did anything succeed" without excluding this message
        // reads its own history as proof the system is healthy - and a message that
        // succeeded once and now fails for ever would never be dead-lettered.
        var classifier = Classifier();
        classifier.RecordOutcome("m-1", succeeded: true);

        var verdict = classifier.Classify(new Attempt("m-1", "TimeoutException", AttemptNumber: 3));

        Assert.Equal(Verdict.Transient, verdict);
    }

    [Fact]
    public void Below_The_Attempt_Threshold_A_Transient_Failure_Stays_Transient()
    {
        // Even when everything else is succeeding: one timeout is one timeout, and the
        // retry it earns is cheap compared with a manual replay.
        var classifier = Classifier();
        classifier.RecordOutcome("m-2", succeeded: true);

        var verdict = classifier.Classify(new Attempt("m-1", "TimeoutException", AttemptNumber: 1));

        Assert.Equal(Verdict.Transient, verdict);
    }

    [Fact]
    public void A_Shape_Failure_Is_Poison_Even_During_A_Full_Outage()
    {
        // Pairs with the outage fact: the system-wide evidence rescues transient failures,
        // and must not rescue a payload that cannot be parsed. That message will still be
        // malformed when the database comes back.
        var classifier = Classifier();
        classifier.RecordOutcome("m-2", succeeded: false);
        classifier.RecordOutcome("m-3", succeeded: false);

        var verdict = classifier.Classify(new Attempt("m-1", "JsonException", AttemptNumber: 1));

        Assert.Equal(Verdict.Poison, verdict);
    }
}

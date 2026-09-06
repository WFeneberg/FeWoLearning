using System.Diagnostics;

namespace FeWoLearning.Telemetry.Exercises.Diagnostics;

// Exercise 019 — KindAndLinks (diagnostics).
// Goal:   Model work that has many causes instead of one, which is what a parent
//         cannot express.
// Drills: ActivityKind Producer and Consumer, ActivityLink, root activities.
// Passes: Enqueue produces one Producer activity and returns its own context;
//         ProcessBatch produces one Consumer activity carrying one link per incoming
//                     context, in order;
//         that batch activity has NO parent - its ParentSpanId is default;
//         and each link's context matches the message it came from, span id included.
//
// The third clause is the entire point and the thing every first attempt gets wrong.
// Parenthood is singular: a span has one parent, because one thing caused it. A batch
// consumer is caused by twenty messages from twenty unrelated traces. Picking one of
// them as the parent is not a simplification, it is a false statement - it grafts the
// batch onto one customer's trace, and the other nineteen traces end at the queue with
// no visible continuation.
//
// A LINK says "related to, not caused by". The batch is its own root, and twenty links
// point back at twenty producers, so every one of those traces can find where its
// message went without any of them owning the batch.
//
// Kind matters for the same reason it is not decoration: a backend uses Producer and
// Consumer to recognise a queue hop and to stop counting the queue's latency as the
// service's own. Get the kind wrong and the shape of every latency chart is wrong.
public static class Ex019_KindAndLinks
{
    /// <summary>The name this exercise's source is registered under.</summary>
    public const string SourceName = "fewolearning.telemetry.ex019";

    /// <summary>The name of the activity that publishes one message.</summary>
    public const string PublishName = "queue.publish";

    /// <summary>The name of the activity that processes a batch.</summary>
    public const string ProcessName = "queue.process";

    /// <summary>The tag on the publish activity carrying the message body.</summary>
    public const string MessageTag = "messaging.body";

    /// <summary>The tag on the batch activity carrying how many messages it took.</summary>
    public const string BatchSizeTag = "messaging.batch.size";

    /// <summary>The one source this exercise emits from.</summary>
    public static ActivitySource Source { get; } = new(SourceName);

    /// <summary>
    /// Publish one message: start a <see cref="PublishName"/> activity of kind
    /// <see cref="ActivityKind.Producer"/>, tag it <see cref="MessageTag"/> with
    /// <paramref name="message"/>, stop it, and return ITS OWN context - the thing a
    /// real producer would write into the message headers.
    ///
    /// Returns <c>default</c> when nothing is listening.
    /// </summary>
    public static ActivityContext Enqueue(string message) =>
        throw new NotImplementedException(
            "TODO: Ex019 - publish as a Producer and hand back the context to travel with the message");

    /// <summary>
    /// Process a batch: start ONE <see cref="ProcessName"/> activity of kind
    /// <see cref="ActivityKind.Consumer"/>, tagged <see cref="BatchSizeTag"/> with the
    /// number of messages, carrying one <see cref="ActivityLink"/> per entry in
    /// <paramref name="incoming"/>, in the same order.
    ///
    /// It must be a ROOT: no parent, whatever any of those contexts say and whatever
    /// happens to be ambient.
    /// </summary>
    public static Activity? ProcessBatch(IReadOnlyList<ActivityContext> incoming) =>
        throw new NotImplementedException(
            "TODO: Ex019 - consume the batch as a parentless Consumer activity linked to every message");
}

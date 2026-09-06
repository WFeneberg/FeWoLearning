using System.Diagnostics;
using OpenTelemetry;

namespace FeWoLearning.Telemetry.Exercises.DesktopOps;

// Exercise 064 — OfflineBufferAndReplay (desktop-ops).
// Goal:   Survive the thing every desktop application does and no server does: run for
//         hours with no network.
// Drills: a custom BaseExporter, a bounded buffer, an explicit drop policy, replay.
// Passes: while the exporter can reach its backend, spans are delivered straight away;
//         while it cannot, nothing is delivered and nothing is lost, up to the capacity;
//         when it can again, the buffer is replayed IN ORDER;
//         past the capacity the OLDEST is dropped, not the newest;
//         and the number dropped is reported, so the gap is known rather than guessed.
//
// A server that cannot reach its collector is a server with a problem. A laptop that
// cannot is a laptop on a train, and it will be back in forty minutes with everything
// that happened in between - which is the interesting part, since whatever the user is
// about to complain about happened while they were offline.
//
// The fourth clause is a real decision and it goes the way most people's instinct does
// not. When the buffer is full you have to lose something, and the newest records are the
// ones nearest to whatever is going wrong NOW - so the oldest go. That is the opposite of
// a queue's natural behaviour, and writing it down as a policy is the difference between
// choosing it and inheriting it.
//
// The fifth is what stops the whole thing being a lie. A buffer that silently drops
// produces telemetry with invisible holes: a gap in a trace that looks like nothing
// happened, in a period where quite a lot did. Counting the drops turns an unknown
// unknown into a number on a dashboard.
//
// And the capacity has to exist at all. An unbounded buffer on a machine with no network
// is not resilience, it is a memory leak with a good excuse - and it takes the
// application down while trying to describe how well it is doing.
public sealed class Ex064_OfflineBufferAndReplay : BaseExporter<Activity>
{
    /// <summary>How many spans may wait. Past this, the oldest go.</summary>
    public const int Capacity = 5;

    /// <summary>Whether the backend can be reached. The test moves this.</summary>
    public bool IsOnline { get; set; } = true;

    /// <summary>Everything that has actually reached the backend, in delivery order.</summary>
    public IReadOnlyList<string> Delivered =>
        throw new NotImplementedException("TODO: Ex064 - expose what has been delivered");

    /// <summary>How many spans are waiting for the backend to come back.</summary>
    public int Buffered =>
        throw new NotImplementedException("TODO: Ex064 - expose how much is waiting");

    /// <summary>How many were lost to the capacity, so the gap is a number.</summary>
    public int Dropped =>
        throw new NotImplementedException("TODO: Ex064 - expose how many were dropped");

    /// <summary>
    /// Take a batch of finished spans.
    ///
    /// Online: deliver them - append each one's <see cref="Activity.DisplayName"/> to
    /// <see cref="Delivered"/>, replaying anything buffered FIRST so order is preserved.
    ///
    /// Offline: buffer them, dropping the oldest and counting it once
    /// <see cref="Capacity"/> is reached.
    /// </summary>
    public override ExportResult Export(in Batch<Activity> batch) =>
        throw new NotImplementedException(
            "TODO: Ex064 - deliver when you can, buffer when you cannot, and count what you drop");
}

using System.Diagnostics.Metrics;

namespace FeWoLearning.Telemetry.Exercises.Diagnostics;

// Exercise 024 — MeterListenerLifecycle (diagnostics).
// Goal:   Write the collecting half yourself, and meet the four ways it silently
//         collects nothing - or collects twice.
// Drills: InstrumentPublished, EnableMeasurementEvents, Start, RecordObservableInstruments,
//         Dispose.
// Passes: measurements from the named meter arrive, and measurements from any other
//                     meter do not;
//         an instrument created AFTER the listener started is picked up too;
//         observable instruments deliver nothing until the listener polls, and then
//                     deliver once per poll;
//         and after the listener is disposed, nothing arrives at all.
//
// Every clause here is a silence, which is what makes this row worth doing by hand.
//
// InstrumentPublished fires once per instrument, including for instruments created long
// after Start - so it is not a one-off census at startup, and a listener that only
// enables what already existed misses everything created later, which in a real app is
// most things.
//
// EnableMeasurementEvents is opt-in PER INSTRUMENT. Being published is not being
// subscribed. Forget the call and the callbacks you carefully wrote never fire.
//
// Start is mandatory and easy to leave out, because everything up to it looks complete.
//
// RecordObservableInstruments is the pull. Observable instruments have no other way to
// report; a listener that never polls shows synchronous instruments only, and a gauge
// that never appears looks exactly like a gauge that was never registered.
//
// And Dispose is the one that costs you money rather than data: a leaked listener keeps
// receiving, so a second one added later means every measurement is counted twice, and
// the dashboards are merely WRONG rather than empty. Empty gets noticed.
public static class Ex024_MeterListenerLifecycle
{
    /// <summary>
    /// Build a <see cref="MeterListener"/>, already started, that:
    ///
    ///   - subscribes to instruments from <paramref name="meterName"/> and no other
    ///     meter, including instruments created after this call;
    ///   - reports every <see cref="long"/> measurement to
    ///     <paramref name="onMeasurement"/> as (instrument name, value);
    ///   - reports observable instruments when, and only when, the caller invokes
    ///     <see cref="MeterListener.RecordObservableInstruments"/>.
    ///
    /// The caller disposes it.
    /// </summary>
    public static MeterListener CreateListener(string meterName, Action<string, long> onMeasurement) =>
        throw new NotImplementedException(
            "TODO: Ex024 - subscribe to one meter's instruments, enable measurements, and start the listener");
}

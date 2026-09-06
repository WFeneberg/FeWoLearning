using FeWoLearning.Telemetry.Exercises.Diagnostics;
using FeWoLearning.Telemetry.Tests.Harness;

namespace FeWoLearning.Telemetry.Tests.Diagnostics;

public class Ex021_MeterAndCounterTests
{
    private static MeasurementProbe Probe() => new(Ex021_MeterAndCounter.MeterName);

    [Fact]
    public void Each_call_adds_one_to_the_counter()
    {
        using var probe = Probe();

        Ex021_MeterAndCounter.RecordProcessed("accepted", "eu-central");
        Ex021_MeterAndCounter.RecordProcessed("accepted", "eu-central");

        var measurements = probe.For(Ex021_MeterAndCounter.InstrumentName);
        Assert.Equal(2, measurements.Count);
        Assert.All(measurements, m => Assert.Equal(1d, m.Value));
    }

    [Fact]
    public void Every_measurement_carries_the_outcome_and_the_region_as_dimensions()
    {
        using var probe = Probe();

        Ex021_MeterAndCounter.RecordProcessed("rejected", "us-east");

        var measurement = Assert.Single(probe.For(Ex021_MeterAndCounter.InstrumentName));
        Assert.Equal("rejected", measurement.Tag(Ex021_MeterAndCounter.OutcomeTag));
        Assert.Equal("us-east", measurement.Tag(Ex021_MeterAndCounter.RegionTag));
    }

    [Fact]
    public void Adversarial_A_The_outcome_is_a_dimension_never_part_of_the_instrument_name()
    {
        // The mistake this prevents is everywhere, because a counter per case is easy
        // to write and reads fine in a list. It is also a dead end: nobody can ask "how
        // many orders in total", since that is a sum over instrument NAMES which no
        // query language addresses; adding a third outcome means shipping code; and
        // every new dimension multiplies the instrument count instead of adding to it.
        using var probe = Probe();

        Ex021_MeterAndCounter.RecordProcessed("accepted", "eu-central");
        Ex021_MeterAndCounter.RecordProcessed("rejected", "eu-central");
        Ex021_MeterAndCounter.RecordProcessed("cancelled", "us-east");

        Assert.Equal([Ex021_MeterAndCounter.InstrumentName], probe.PublishedInstruments);
    }

    [Fact]
    public void Adversarial_B_One_instrument_still_separates_the_outcomes()
    {
        // The paired use fact. Collapsing everything onto one instrument would satisfy
        // Adversarial_A perfectly and lose the very thing the dimensions are for.
        using var probe = Probe();

        Ex021_MeterAndCounter.RecordProcessed("accepted", "eu-central");
        Ex021_MeterAndCounter.RecordProcessed("accepted", "eu-central");
        Ex021_MeterAndCounter.RecordProcessed("rejected", "eu-central");

        var byOutcome = probe.For(Ex021_MeterAndCounter.InstrumentName)
            .GroupBy(m => m.Tag(Ex021_MeterAndCounter.OutcomeTag))
            .ToDictionary(g => g.Key!, g => g.Sum(m => m.Value));

        Assert.Equal(2d, byOutcome["accepted"]);
        Assert.Equal(1d, byOutcome["rejected"]);
    }

    [Fact]
    public void The_instrument_declares_its_unit()
    {
        // A number without a unit is a number somebody will eventually plot next to a
        // different unit. UCUM's curly-brace form is how you say "a plain count of
        // things" rather than a physical quantity.
        using var probe = Probe();

        Ex021_MeterAndCounter.RecordProcessed("accepted", "eu-central");

        Assert.Equal(
            Ex021_MeterAndCounter.InstrumentUnit,
            probe.UnitOf(Ex021_MeterAndCounter.InstrumentName));
    }
}

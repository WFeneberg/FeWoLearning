using FeWoLearning.Architecture.Exercises.Domain.Ex089;
using FeWoLearning.Architecture.Exercises.Support;

namespace FeWoLearning.Architecture.Tests.Domain;

public class Ex089_WebhookDeliveryTests
{
    private const string Secret = "whsec_correct-horse-battery-staple";
    private const string Body = """{"event":"order.placed","id":"o-1"}""";
    private static readonly TimeSpan Tolerance = TimeSpan.FromMinutes(5);

    private static ManualClock Clock() => new(new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero));

    [Fact]
    public void A_Signed_Delivery_Verifies()
    {
        var clock = Clock();
        var delivery = Ex089_WebhookDelivery.Sign(clock, Secret, "evt-1", Body);

        Assert.Null(Record.Exception(() => Ex089_WebhookDelivery.Verify(clock, Secret, delivery, Tolerance)));
    }

    [Fact]
    public void A_Tampered_Body_Is_Refused()
    {
        var clock = Clock();
        var delivery = Ex089_WebhookDelivery.Sign(clock, Secret, "evt-1", Body);

        var tampered = delivery with { Body = """{"event":"order.placed","id":"o-999"}""" };

        Assert.Throws<WebhookRejectedException>(() => Ex089_WebhookDelivery.Verify(clock, Secret, tampered, Tolerance));
    }

    [Fact]
    public void The_Wrong_Secret_Is_Refused()
    {
        var clock = Clock();
        var delivery = Ex089_WebhookDelivery.Sign(clock, Secret, "evt-1", Body);

        Assert.Throws<WebhookRejectedException>(
            () => Ex089_WebhookDelivery.Verify(clock, "whsec_someone-elses", delivery, Tolerance));
    }

    [Fact]
    public void Mechanism_The_Timestamp_Is_Inside_The_Signature()
    {
        // Signing the body alone leaves the replay window unenforceable: an attacker sends
        // the old body with a fresh timestamp header and the signature still checks out.
        // Swapping the timestamp must therefore break the signature, not merely move the
        // delivery back inside the window.
        var clock = Clock();
        var delivery = Ex089_WebhookDelivery.Sign(clock, Secret, "evt-1", Body);

        clock.Advance(Tolerance * 2);
        var refreshed = delivery with { TimestampSeconds = clock.UtcNow.ToUnixTimeSeconds() };

        var failure = Assert.Throws<WebhookRejectedException>(
            () => Ex089_WebhookDelivery.Verify(clock, Secret, refreshed, Tolerance));

        Assert.Contains("Signature", failure.Message);
    }

    [Fact]
    public void Mechanism_A_Delivery_Older_Than_The_Tolerance_Is_Refused()
    {
        // A signature proves WHO sent it and never WHEN. Without the window, anybody who
        // ever captured one request can send it again for ever and it stays valid.
        var clock = Clock();
        var delivery = Ex089_WebhookDelivery.Sign(clock, Secret, "evt-1", Body);

        clock.Advance(Tolerance + TimeSpan.FromSeconds(1));

        var failure = Assert.Throws<WebhookRejectedException>(
            () => Ex089_WebhookDelivery.Verify(clock, Secret, delivery, Tolerance));

        Assert.Contains("tolerance", failure.Message);
    }

    [Fact]
    public void A_Delivery_Inside_The_Tolerance_Is_Accepted()
    {
        // Paired with the fact above: the window must not be so strict that ordinary
        // network latency, or a receiver whose clock is a few seconds off, rejects
        // everything.
        var clock = Clock();
        var delivery = Ex089_WebhookDelivery.Sign(clock, Secret, "evt-1", Body);

        clock.Advance(Tolerance - TimeSpan.FromSeconds(1));

        Assert.Null(Record.Exception(() => Ex089_WebhookDelivery.Verify(clock, Secret, delivery, Tolerance)));
    }

    [Fact]
    public void Mechanism_The_Receiver_Handles_An_Event_Once()
    {
        // The sender retries on anything it cannot distinguish from a timeout, so
        // at-least-once is the guarantee whether anybody chose it or not.
        var clock = Clock();
        var receiver = new WebhookReceiver(clock, Secret, Tolerance);
        var delivery = Ex089_WebhookDelivery.Sign(clock, Secret, "evt-1", Body);

        Assert.True(receiver.Receive(delivery));
        Assert.False(receiver.Receive(delivery));
        Assert.Equal(1, receiver.Handled);
    }

    [Fact]
    public void Adversarial_An_Unsigned_Delivery_Cannot_Suppress_The_Genuine_One()
    {
        // Recording the event id before verifying lets anybody who knows an id post
        // garbage under it and permanently silence the real delivery that follows - a
        // denial of service that needs no secret at all.
        var clock = Clock();
        var receiver = new WebhookReceiver(clock, Secret, Tolerance);
        var genuine = Ex089_WebhookDelivery.Sign(clock, Secret, "evt-1", Body);
        var forged = genuine with { Signature = new string('0', genuine.Signature.Length) };

        Assert.Throws<WebhookRejectedException>(() => receiver.Receive(forged));

        Assert.True(receiver.Receive(genuine));
        Assert.Equal(1, receiver.Handled);
    }

    [Fact]
    public void Different_Events_Are_Both_Handled()
    {
        var clock = Clock();
        var receiver = new WebhookReceiver(clock, Secret, Tolerance);

        Assert.True(receiver.Receive(Ex089_WebhookDelivery.Sign(clock, Secret, "evt-1", Body)));
        Assert.True(receiver.Receive(Ex089_WebhookDelivery.Sign(clock, Secret, "evt-2", Body)));

        Assert.Equal(2, receiver.Handled);
    }
}

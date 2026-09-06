using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace FeWoLearning.Telemetry.Exercises.Otel;

// Exercise 028 — ResourceAttributes (otel-sdk).
// Goal:   Say WHO is emitting, once, for every span, metric and log the process ever
//         produces.
// Drills: ResourceBuilder, service.name / service.version / service.instance.id,
//         environment-variable detection.
// Passes: BuildResource carries service.name, service.version and service.instance.id
//                     with the values given;
//         a provider built from it reports the same resource;
//         the environment detector picks up OTEL_RESOURCE_ATTRIBUTES, so an operator
//                     can add attributes the code never mentioned;
//         and an attribute set in code is not lost when the environment adds others.
//
// A resource is not a tag. A tag varies per span; a resource is constant for the whole
// process and is attached by the exporter, once, to everything - so it is where
// "which service, which version, which instance" belongs, and putting those on spans
// instead multiplies the stored data by the number of spans for no benefit.
//
// service.name is the one attribute every backend requires. Omit it and the OTel SDK
// substitutes "unknown_service:<processname>", which is not an error, is not a warning,
// and is exactly what you will find in the dropdown three weeks later when you go
// looking for your service.
//
// service.instance.id is the one people skip, and it is the one that matters at three
// in the morning: without it, every replica's metrics are averaged together and the one
// unhealthy pod is invisible.
//
// The environment detector is what keeps deployment concerns out of the code. The
// operator sets OTEL_RESOURCE_ATTRIBUTES=deployment.environment=staging on the
// container; nothing is rebuilt, and nothing in the source has to know that staging
// exists.
public static class Ex028_ResourceAttributes
{
    /// <summary>The service this process is.</summary>
    public const string ServiceName = "fewolearning-telemetry";

    /// <summary>The build it is running.</summary>
    public const string ServiceVersion = "1.4.0";

    /// <summary>The conventional attribute keys. Spelling is not negotiable.</summary>
    public const string ServiceNameKey = "service.name";

    /// <inheritdoc cref="ServiceNameKey"/>
    public const string ServiceVersionKey = "service.version";

    /// <inheritdoc cref="ServiceNameKey"/>
    public const string ServiceInstanceIdKey = "service.instance.id";

    /// <summary>The variable an operator sets on the container.</summary>
    public const string ResourceAttributesVariable = "OTEL_RESOURCE_ATTRIBUTES";

    /// <summary>The source this exercise emits from.</summary>
    public static ActivitySource Source { get; } = new("fewolearning.telemetry.ex028");

    /// <summary>
    /// Build the resource identifying this process:
    /// <see cref="ServiceName"/>, <see cref="ServiceVersion"/> and
    /// <paramref name="instanceId"/> - and also whatever the environment adds through
    /// <see cref="ResourceAttributesVariable"/>.
    ///
    /// Start from an EMPTY resource, so the result contains what this method put there
    /// and nothing the SDK guessed.
    /// </summary>
    public static Resource BuildResource(string instanceId) =>
        throw new NotImplementedException(
            "TODO: Ex028 - build a resource naming the service, its version and this instance, plus the environment's");

    /// <summary>
    /// Build a provider that exports into <paramref name="exported"/> and carries
    /// <see cref="BuildResource"/>'s resource.
    ///
    /// The caller disposes it.
    /// </summary>
    public static TracerProvider Build(ICollection<Activity> exported, string instanceId) =>
        throw new NotImplementedException("TODO: Ex028 - build a provider carrying this process's resource");
}

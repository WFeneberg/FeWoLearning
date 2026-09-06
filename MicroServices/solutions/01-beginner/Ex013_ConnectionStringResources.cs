using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

namespace FeWoLearning.MicroServices.Exercises.Beginner;

/// <summary>
/// Goal:   Bring a store Aspire does NOT host - a legacy Oracle box owned by another
///         team - into the model, so consumers reference it exactly the way they
///         reference a store Aspire does host.
/// Drills: `AddConnectionString(name, ReferenceExpression)`, ConnectionStringResource,
///         and the publish-time split: an external store is value.v0 (a connection
///         string and nothing else), a hosted store is container.v0 (an image, an
///         environment, bindings). The secret half stays a parameter rather than
///         being baked into the literal.
/// Passes: "legacy" is a ConnectionStringResource whose expression interpolates
///         {legacy-password.value} rather than a plaintext password; the manifest
///         gives "legacy" type value.v0 and "cache" type container.v0; and
///         "reporting" receives ConnectionStrings__legacy as the unresolved
///         reference {legacy.connectionString}.
/// Note:   Measured on 13.5.3 - the OTHER overloads do not do this. Both
///         AddConnectionString("legacy") and AddConnectionString("legacy",
///         "LEGACY_CONNECTION") produce an internal ConnectionStringParameterResource
///         that publishes as parameter.v0, not value.v0, and a test cannot even name
///         that type (CS0122). The ReferenceExpression overload - and the equivalent
///         AddConnectionString(name, builder => builder.Append($"...")) - is the one
///         this row is about.
/// </summary>
public static class Ex013_ConnectionStringResources
{
    public static void Configure(IDistributedApplicationBuilder builder)
    {
        // The secret half stays a parameter, so the password never appears in the
        // model, in the manifest or in source control - the expression below
        // interpolates a REFERENCE to it.
        var password = builder.AddParameter("legacy-password", secret: true);

        // Nothing here is started, pulled or built. AddConnectionString states that a
        // store exists somewhere else and describes how to reach it; the resource is
        // still a first-class IResourceWithConnectionString, so consumers reference
        // it with exactly the WithReference call they would use for a hosted store.
        var legacy = builder.AddConnectionString("legacy", ReferenceExpression.Create(
            $"Data Source=legacy-oracle.corp:1521/ORCL;User Id=reporting;Password={password}"));

        // A store Aspire DOES host, for contrast: this one publishes as container.v0.
        builder.AddContainer("cache", "redis");

        builder.AddContainer("reporting", "busybox")
               .WithReference(legacy);
    }
}

using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

namespace FeWoLearning.MicroServices.Exercises.Beginner;

/// <summary>
/// Goal:   Extend the application model with a fact Aspire has never heard of - which
///         team owns a resource - and hand it to the rest of the AppHost through a
///         fluent `WithOwningTeam(...)` call that reads like every built-in one.
/// Drills: Writing an IResourceAnnotation, writing an IResourceBuilder&lt;T&gt; extension
///         over `WithAnnotation`, and reading one back with `TryGetLastAnnotation`.
///         The subject underneath is why a resource's Annotations is a LIST and not a
///         dictionary keyed by type: annotations accumulate, so two calls leave two
///         records and the newest wins by position rather than by overwriting the
///         older one. That is what makes an audit trail (who claimed this resource,
///         and in what order) possible at all.
/// Passes: "api" carries TWO OwningTeamAnnotations - "payments" then "billing", in
///         that order - and TryGetLastAnnotation returns "billing"; "pg" carries
///         exactly one, "platform", which proves the extension is generic over T
///         rather than hard-wired to containers; and "cache", which nobody claimed,
///         carries none at all.
/// Note:   WithAnnotation's default mutation behaviour is Append. The plausible wrong
///         answer is WithAnnotation(annotation, ResourceAnnotationMutationBehavior
///         .Replace), which is the dictionary-shaped mental model: measured on 13.5.3
///         it leaves "api" with ONE annotation, "billing". Everything else about the
///         model looks identical, and TryGetLastAnnotation still answers "billing" -
///         so only counting the annotations tells the two apart. "cache" is
///         declared FIRST for a second, measured reason: an extension that annotates
///         every resource it can reach rather than the one it was chained onto is
///         invisible if the unclaimed resource is declared last.
/// </summary>
public static class Ex016_CustomAnnotationsAndExtensions
{
    public static void Configure(IDistributedApplicationBuilder builder)
    {
        // Nobody claims the cache, and it is declared FIRST on purpose: an
        // implementation that annotates every resource it can reach instead of the
        // one the call was chained onto only gets caught if the unclaimed resource
        // already exists while the claims are being made.
        builder.AddContainer("cache", "redis");

        // Two claims on one resource. Both survive, in call order, because
        // WithAnnotation appends - this is the whole point of the row.
        builder.AddContainer("api", "nginx")
               .WithOwningTeam("payments")
               .WithOwningTeam("billing");

        // A Postgres server is not a ContainerResource as far as the C# type system
        // is concerned, so this line only compiles if WithOwningTeam is generic over
        // T the way Aspire's own WithX extensions are.
        builder.AddPostgres("pg")
               .WithOwningTeam("platform");
    }
}

/// <summary>
/// A custom annotation. IResourceAnnotation is a marker interface with no members:
/// any type implementing it can ride along on a resource, and Aspire will carry it
/// through the model untouched. Immutable, because a model that can be rewritten
/// after the fact is a model nothing can be asserted about.
/// </summary>
public sealed class OwningTeamAnnotation : IResourceAnnotation
{
    public OwningTeamAnnotation(string team) => Team = team;

    public string Team { get; }
}

/// <summary>
/// The fluent extension. Generic over T so it reaches any resource - containers,
/// projects, Postgres servers, parameters - exactly the way Aspire's own WithX
/// methods do, and returning the builder so calls chain.
/// </summary>
public static class OwningTeamResourceBuilderExtensions
{
    public static IResourceBuilder<T> WithOwningTeam<T>(this IResourceBuilder<T> builder, string team)
        where T : IResource
        // No ResourceAnnotationMutationBehavior argument, so this is Append. Passing
        // Replace here would make the second claim erase the first and turn the
        // annotation list into a one-slot dictionary.
        => builder.WithAnnotation(new OwningTeamAnnotation(team));
}

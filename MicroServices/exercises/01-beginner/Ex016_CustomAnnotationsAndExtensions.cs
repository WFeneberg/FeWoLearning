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
        => throw new NotImplementedException(
            "TODO: ex016 - add a container \"cache\" on image \"redis\" and claim "
            + "it for nobody; add a container \"api\" on image \"nginx\" and claim "
            + "it first for team \"payments\" and then for team \"billing\"; add a "
            + "Postgres server \"pg\" and claim it for team \"platform\". Declare "
            + "them in that order, and use WithOwningTeam below for every claim.");
}

/// <summary>
/// TODO: ex016 - a custom annotation. An IResourceAnnotation is just a marker
/// interface: any type that implements it can ride along on a resource. Store the
/// team name and expose it, immutably.
/// </summary>
public sealed class OwningTeamAnnotation : IResourceAnnotation
{
    public OwningTeamAnnotation(string team)
        => throw new NotImplementedException("TODO: ex016 - remember the team name.");

    public string Team
        => throw new NotImplementedException("TODO: ex016 - hand back the team name.");
}

/// <summary>
/// TODO: ex016 - the fluent extension. Generic over T so it reaches any resource -
/// containers, projects, Postgres servers, parameters - exactly the way Aspire's own
/// WithX methods do, and returning the builder so calls chain.
/// </summary>
public static class OwningTeamResourceBuilderExtensions
{
    public static IResourceBuilder<T> WithOwningTeam<T>(this IResourceBuilder<T> builder, string team)
        where T : IResource
        => throw new NotImplementedException(
            "TODO: ex016 - attach an OwningTeamAnnotation carrying `team` via "
            + "WithAnnotation, and return the builder. Do NOT replace an annotation "
            + "that is already there - claims accumulate.");
}

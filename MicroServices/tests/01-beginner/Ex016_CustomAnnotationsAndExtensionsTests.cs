using Aspire.Hosting.ApplicationModel;
using FeWoLearning.MicroServices.Exercises.Beginner;

namespace FeWoLearning.MicroServices.Tests.Beginner;

public class Ex016_CustomAnnotationsAndExtensionsTests
{
    private static IReadOnlyList<string> TeamsOf(IResource resource)
        => resource.Annotations.OfType<OwningTeamAnnotation>().Select(a => a.Team).ToList();

    [Fact]
    public void The_custom_annotation_reaches_the_resource_the_extension_was_called_on()
    {
        var model = ModelHarness.Build(Ex016_CustomAnnotationsAndExtensions.Configure);

        // The annotation, not an environment variable and not a naming convention.
        // WithEnvironment("OWNING_TEAM", "payments") would carry the same string
        // through the model and would leave ZERO OwningTeamAnnotations here, which is
        // the whole difference between extending the model and decorating a process.
        Assert.Contains("payments", TeamsOf(model.Resource("api")));

        // "pg" is a PostgresServerResource, not a ContainerResource. Claiming it at
        // all is what proves WithOwningTeam is generic over T rather than typed to
        // IResourceBuilder<ContainerResource> - a non-generic version cannot even
        // compile the solution's second call.
        Assert.IsType<PostgresServerResource>(model.Resource("pg"));
        Assert.Equal(["platform"], TeamsOf(model.Resource("pg")));
    }

    [Fact]
    public void Annotations_are_a_list_so_a_second_claim_appends_instead_of_replacing()
    {
        var model = ModelHarness.Build(Ex016_CustomAnnotationsAndExtensions.Configure);
        var api = model.Resource("api");

        // The grading fact of this row. WithAnnotation's default behaviour is Append;
        // measured on 13.5.3, passing ResourceAnnotationMutationBehavior.Replace
        // instead leaves exactly ONE annotation reading "billing". That mutant is the
        // dictionary-shaped mental model the catalog row exists to reject, and it is
        // invisible to every other assertion here - including the TryGetLastAnnotation
        // one below, which still answers "billing" against it. Order is asserted too,
        // because a set-equality check would grade nothing about accumulation.
        Assert.Equal(["payments", "billing"], TeamsOf(api));

        // TryGetLastAnnotation is the read side of "it is a list": LAST, not "the
        // one", and it is how the rest of an AppHost consumes a custom annotation
        // without knowing how many were attached.
        Assert.True(api.TryGetLastAnnotation<OwningTeamAnnotation>(out var last));
        Assert.Equal("billing", last!.Team);
    }

    [Fact]
    public void A_resource_nobody_claimed_carries_no_annotation_at_all()
    {
        var model = ModelHarness.Build(Ex016_CustomAnnotationsAndExtensions.Configure);

        // The negative half, and it is not decoration: an implementation that parked
        // the team in a static field, or that annotated builder.Resources wholesale
        // instead of the one resource the call was chained onto, passes both facts
        // above and fails here.
        Assert.False(model.Resource("cache").TryGetLastAnnotation<OwningTeamAnnotation>(out var none));
        Assert.Null(none);
        Assert.Empty(TeamsOf(model.Resource("cache")));

        // And the claims did not bleed sideways: "pg" answers "platform", not the
        // "billing" that was the most recent call in the file.
        Assert.True(model.Resource("pg").TryGetLastAnnotation<OwningTeamAnnotation>(out var pg));
        Assert.Equal("platform", pg!.Team);
    }
}

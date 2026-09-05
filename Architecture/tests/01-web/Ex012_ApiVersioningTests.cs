using FeWoLearning.Architecture.Exercises.Web.Ex012;

namespace FeWoLearning.Architecture.Tests.Web;

public class Ex012_ApiVersioningTests
{
    private static readonly Order Sample = new("O-1", Subtotal: 100m, Tax: 19m);

    [Fact]
    public void V1_Renders_The_Original_Contract()
    {
        var rendered = Ex012_ApiVersioning.Render(Sample, 1);

        var v1 = Assert.IsType<OrderV1>(rendered);
        Assert.Equal("O-1", v1.Id);
        Assert.Equal(119m, v1.Total);
    }

    [Fact]
    public void V2_Renders_The_Broken_Out_Contract()
    {
        var rendered = Ex012_ApiVersioning.Render(Sample, 2);

        var v2 = Assert.IsType<OrderV2>(rendered);
        Assert.Equal(100m, v2.Subtotal);
        Assert.Equal(19m, v2.Tax);
        Assert.Equal(119m, v2.Total);
    }

    [Fact]
    public void Adversarial_V1_Did_Not_Grow_V2s_Fields()
    {
        // The failure mode this exercise exists for. Adding Subtotal and Tax to the
        // EXISTING contract looks additive and passes both facts above; it breaks every
        // v1 client that validates its response against a closed schema. Asserting the
        // returned type is exactly OrderV1 is what pins the two contracts apart.
        var rendered = Ex012_ApiVersioning.Render(Sample, 1);

        Assert.IsType<OrderV1>(rendered, exactMatch: true);
        Assert.Null(rendered.GetType().GetProperty("Tax"));
        Assert.Null(rendered.GetType().GetProperty("Subtotal"));
    }

    [Fact]
    public void Both_Versions_Agree_On_The_Total_Because_There_Is_One_Model()
    {
        // Two contracts, one model. An implementation that recomputed the total
        // per-version is where the two silently drift apart by a rounding rule.
        var v1 = Assert.IsType<OrderV1>(Ex012_ApiVersioning.Render(Sample, 1));
        var v2 = Assert.IsType<OrderV2>(Ex012_ApiVersioning.Render(Sample, 2));

        Assert.Equal(v1.Total, v2.Total);
    }

    [Fact]
    public void An_Unknown_Version_Is_Rejected_By_Name()
    {
        var failure = Assert.Throws<NotSupportedException>(() => Ex012_ApiVersioning.Render(Sample, 3));

        Assert.Contains("3", failure.Message);
    }
}

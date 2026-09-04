using FeWoLearning.Uno.Exercises.Expert;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Tests.Expert;

public class Ex100_FeatureModulesTests : UnoTestContext
{
    private interface IOrdersService;

    private sealed class OrdersService : IOrdersService;

    private interface IBillingService;

    private sealed class BillingService : IBillingService;

    private sealed class OrdersModule : IEx100_FeatureModule
    {
        public string Name => "orders";

        public void RegisterServices(IServiceCollection services) =>
            services.AddSingleton<IOrdersService, OrdersService>();

        public void RegisterRoutes(IDictionary<string, Type> routes) => routes["orders"] = typeof(Ex094_HomePage);
    }

    private sealed class BillingModule : IEx100_FeatureModule
    {
        public string Name => "billing";

        public void RegisterServices(IServiceCollection services) =>
            services.AddSingleton<IBillingService, BillingService>();

        public void RegisterRoutes(IDictionary<string, Type> routes) => routes["billing"] = typeof(Ex094_DetailPage);
    }

    private sealed class BrokenModule : IEx100_FeatureModule
    {
        public string Name => "broken";

        public void RegisterServices(IServiceCollection services) => throw new InvalidOperationException("half written");

        public void RegisterRoutes(IDictionary<string, Type> routes) => routes["broken"] = typeof(Ex094_HomePage);
    }

    [Fact]
    public void A_Module_Contributes_Its_Services()
    {
        var shell = Ex100_FeatureModules.Compose(new OrdersModule());

        Assert.NotNull(shell.Services.GetService<IOrdersService>());
    }

    [Fact]
    public void A_Module_Contributes_Its_Routes()
    {
        var shell = Ex100_FeatureModules.Compose(new OrdersModule());

        Assert.Equal(typeof(Ex094_HomePage), Ex100_FeatureModules.Resolve(shell, "orders"));
    }

    [Fact]
    public void Several_Modules_All_Contribute()
    {
        var shell = Ex100_FeatureModules.Compose(new OrdersModule(), new BillingModule());

        Assert.NotNull(shell.Services.GetService<IOrdersService>());
        Assert.NotNull(shell.Services.GetService<IBillingService>());
        Assert.Equal(2, shell.Routes.Count);
    }

    [Fact]
    public void The_Order_Of_The_Modules_Does_Not_Matter()
    {
        var forwards = Ex100_FeatureModules.Compose(new OrdersModule(), new BillingModule());
        var backwards = Ex100_FeatureModules.Compose(new BillingModule(), new OrdersModule());

        Assert.Equal(forwards.Routes.Keys.Order(), backwards.Routes.Keys.Order());
    }

    [Fact]
    public void The_Loaded_Modules_Are_Named()
    {
        var shell = Ex100_FeatureModules.Compose(new OrdersModule(), new BillingModule());

        Assert.Equal(["orders", "billing"], shell.Loaded);
        Assert.Empty(shell.Failed);
    }

    [Fact]
    public void A_Broken_Module_Is_Recorded_Rather_Than_Fatal()
    {
        var shell = Ex100_FeatureModules.Compose(new BrokenModule());

        Assert.Contains("broken", shell.Failed.Keys);
        Assert.Contains("half written", shell.Failed["broken"]);
        Assert.Empty(shell.Loaded);
    }

    [Fact]
    public void A_Broken_Module_Does_Not_Stop_The_Others()
    {
        var shell = Ex100_FeatureModules.Compose(new OrdersModule(), new BrokenModule(), new BillingModule());

        // In a real app the broken module is the one somebody is still writing, and the
        // shell has to come up anyway.
        Assert.Equal(["orders", "billing"], shell.Loaded);
        Assert.NotNull(shell.Services.GetService<IBillingService>());
    }

    [Fact]
    public void An_Unknown_Route_Resolves_To_Nothing()
    {
        var shell = Ex100_FeatureModules.Compose(new OrdersModule());

        Assert.Null(Ex100_FeatureModules.Resolve(shell, "nowhere"));
    }

    [Fact]
    public void A_Shell_With_No_Modules_Still_Works()
    {
        var shell = Ex100_FeatureModules.Compose();

        Assert.Empty(shell.Routes);
        Assert.Empty(shell.Loaded);
        Assert.NotNull(shell.Services);
    }

    [Fact]
    public void The_Shell_Names_No_Feature()
    {
        var shell = Ex100_FeatureModules.Compose(new OrdersModule(), new BillingModule());

        // Nothing in Compose mentions orders or billing: adding a feature is adding a
        // file, and removing one is removing a file - which is the harder half.
        Assert.Equal(2, shell.Loaded.Count);
        Assert.All(shell.Routes.Values, pageType => Assert.True(typeof(Page).IsAssignableFrom(pageType)));
    }
}

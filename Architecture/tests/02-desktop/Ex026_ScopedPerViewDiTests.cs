using FeWoLearning.Architecture.Exercises.Desktop.Ex026;
using Microsoft.Extensions.DependencyInjection;

namespace FeWoLearning.Architecture.Tests.Desktop;

public class Ex026_ScopedPerViewDiTests
{
    private static ServiceProvider Root()
    {
        var services = new ServiceCollection();
        services.AddScoped<ViewLocalService>();
        services.AddSingleton<SharedService>();
        return services.BuildServiceProvider();
    }

    [Fact]
    public void Mechanism_Two_Open_Views_Get_Their_Own_Instances()
    {
        // Handing back the root provider passes nothing here, and is exactly what a
        // desktop application does by accident when it treats the container the way a
        // web request does.
        using var root = Root();
        using var manager = new ViewScopeManager(root);

        var first = manager.OpenView("orders").GetRequiredService<ViewLocalService>();
        var second = manager.OpenView("customers").GetRequiredService<ViewLocalService>();

        Assert.NotSame(first, second);
    }

    [Fact]
    public void One_View_Resolves_The_Same_Instance_Every_Time()
    {
        using var root = Root();
        using var manager = new ViewScopeManager(root);

        var view = manager.OpenView("orders");

        Assert.Same(view.GetRequiredService<ViewLocalService>(), view.GetRequiredService<ViewLocalService>());
    }

    [Fact]
    public void Singletons_Are_Still_Shared_Across_Views()
    {
        // A scope is not a new container. Getting this wrong - by building a second
        // provider per view instead of a child scope - duplicates every singleton, and
        // with them every cache and connection pool the application owns.
        using var root = Root();
        using var manager = new ViewScopeManager(root);

        var first = manager.OpenView("orders").GetRequiredService<SharedService>();
        var second = manager.OpenView("customers").GetRequiredService<SharedService>();

        Assert.Same(first, second);
    }

    [Fact]
    public void Closing_A_View_Disposes_Its_Services_Exactly_Once()
    {
        using var root = Root();
        using var manager = new ViewScopeManager(root);
        var service = manager.OpenView("orders").GetRequiredService<ViewLocalService>();

        manager.CloseView("orders");

        Assert.Equal(1, service.DisposeCount);
    }

    [Fact]
    public void Mechanism_Closing_One_View_Leaves_The_Other_Alone()
    {
        // A manager that disposed the root provider, or kept one shared scope, passes
        // the disposal fact above and takes the user's other open windows down with it.
        using var root = Root();
        using var manager = new ViewScopeManager(root);
        var closing = manager.OpenView("orders").GetRequiredService<ViewLocalService>();
        var staying = manager.OpenView("customers").GetRequiredService<ViewLocalService>();

        manager.CloseView("orders");

        Assert.Equal(1, closing.DisposeCount);
        Assert.Equal(0, staying.DisposeCount);
    }

    [Fact]
    public void Adversarial_Closing_Twice_Does_Not_Dispose_Twice()
    {
        // Closing a window that is already closing is ordinary in a UI - a Closed event
        // and an explicit teardown call both arrive. Disposing twice runs every
        // IDisposable a second time, and the second run is the one that throws on a
        // connection that is already returned to the pool.
        using var root = Root();
        using var manager = new ViewScopeManager(root);
        var service = manager.OpenView("orders").GetRequiredService<ViewLocalService>();

        manager.CloseView("orders");
        manager.CloseView("orders");

        Assert.Equal(1, service.DisposeCount);
    }

    [Fact]
    public void Opening_The_Same_View_Twice_Is_Refused()
    {
        using var root = Root();
        using var manager = new ViewScopeManager(root);
        manager.OpenView("orders");

        Assert.Throws<InvalidOperationException>(() => manager.OpenView("orders"));
    }
}

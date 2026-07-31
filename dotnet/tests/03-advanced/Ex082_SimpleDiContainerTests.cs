using System;
using FeWoLearning.Exercises.Advanced;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Advanced;

public class Ex082_SimpleDiContainerTests
{
    public interface IGreeter
    {
        string Greet();
    }

    public sealed class EnglishGreeter : IGreeter
    {
        public string Greet() => "Hello";
    }

    public interface IRepository
    {
        Guid InstanceId { get; }
    }

    public sealed class Repository : IRepository
    {
        public Guid InstanceId { get; } = Guid.NewGuid();
    }

    public interface ILogger
    {
        string Prefix { get; }
    }

    public sealed class ConsoleLogger : ILogger
    {
        public string Prefix => "[log]";
    }

    public interface IReportService
    {
        string Describe();
    }

    // Depends on ILogger — exercises constructor injection of a registered dependency.
    public sealed class ReportService : IReportService
    {
        private readonly ILogger _logger;

        public ReportService(ILogger logger) => _logger = logger;

        public string Describe() => $"{_logger.Prefix} report";
    }

    [Fact]
    public void Resolve_ReturnsInstanceImplementingRegisteredInterface()
    {
        var container = new SimpleDiContainer();
        container.Register<IGreeter, EnglishGreeter>();

        var greeter = container.Resolve<IGreeter>();

        Assert.IsAssignableFrom<IGreeter>(greeter);
        Assert.IsType<EnglishGreeter>(greeter);
        Assert.Equal("Hello", greeter.Greet());
    }

    [Fact]
    public void Resolve_TransientRegistration_ReturnsDistinctInstances()
    {
        var container = new SimpleDiContainer();
        container.Register<IRepository, Repository>();

        var first = container.Resolve<IRepository>();
        var second = container.Resolve<IRepository>();

        Assert.NotSame(first, second);
        Assert.NotEqual(first.InstanceId, second.InstanceId);
    }

    [Fact]
    public void Resolve_SingletonRegistration_ReturnsSameInstance()
    {
        var container = new SimpleDiContainer();
        container.Register<IRepository, Repository>(singleton: true);

        var first = container.Resolve<IRepository>();
        var second = container.Resolve<IRepository>();

        Assert.Same(first, second);
        Assert.Equal(first.InstanceId, second.InstanceId);
    }

    [Fact]
    public void Resolve_InjectsRegisteredConstructorDependencies()
    {
        var container = new SimpleDiContainer();
        container.Register<ILogger, ConsoleLogger>(singleton: true);
        container.Register<IReportService, ReportService>();

        var service = container.Resolve<IReportService>();

        Assert.IsAssignableFrom<IReportService>(service);
        Assert.Equal("[log] report", service.Describe());
    }

    [Fact]
    public void IsRegistered_ReflectsRegistrationState()
    {
        var container = new SimpleDiContainer();

        Assert.False(container.IsRegistered<IGreeter>());
        container.Register<IGreeter, EnglishGreeter>();
        Assert.True(container.IsRegistered<IGreeter>());
    }

    [Fact]
    public void Resolve_UnregisteredInterface_ThrowsInvalidOperationException()
    {
        var container = new SimpleDiContainer();
        Assert.Throws<InvalidOperationException>(() => container.Resolve<IGreeter>());
    }
}

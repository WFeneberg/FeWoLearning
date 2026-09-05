// Exercise 030 - Simple Container Instances (beginner).
// Goal:   Learn the other two ways to tell SimpleContainer what to hand back: a ready-made
//         object (RegisterInstance) and a factory delegate (RegisterHandler) - and that asking
//         for every registration under one service, not just the latest, is a first-class query.
// Drills: SimpleContainer.RegisterInstance returning the EXACT object handed to it, not a copy or
//         a new instance built from it; RegisterHandler's factory running again on every
//         resolution, not just the first; GetAllInstances counting every registration for a
//         service, duplicates and mixed registration kinds included - registering the same
//         service a second time never overwrites the first registration.
// Passes: dotnet test --filter FullyQualifiedName~Ex030_

using System.Linq;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex030_SimpleContainerInstances
{
    public void RegisterInstance<TService>(SimpleContainer container, TService instance) where TService : class =>
        container.RegisterInstance(typeof(TService), null, instance);

    public void RegisterHandler<TService>(SimpleContainer container, Func<SimpleContainer, object> factory) =>
        container.RegisterHandler(typeof(TService), null, factory);

    public int CountRegistrations<TService>(SimpleContainer container) =>
        container.GetAllInstances(typeof(TService), null).Count();
}

/// <summary>A service with an identity you can compare across resolutions.</summary>
public interface IEx030_Thing
{
    Guid Id { get; }
}

public class Ex030_Thing : IEx030_Thing
{
    public Guid Id { get; } = Guid.NewGuid();
}

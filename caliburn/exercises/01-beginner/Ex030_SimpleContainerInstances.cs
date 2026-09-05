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
//
// Measured on this machine (Caliburn.Micro 5.0.258): container.RegisterInstance(typeof(IThing),
// null, someObject) then GetInstance(typeof(IThing), null) returns THAT VERY OBJECT - reference-
// equal, not merely equal-by-value, and every further resolution returns the same object again.
// container.RegisterHandler(typeof(IThing), null, factory), a Func<SimpleContainer, object>, runs
// the factory again on every single GetInstance call - two resolutions measured two factory
// invocations, and two different returned objects, even though only one handler was ever
// registered. Registering the SAME service twice (RegisterPerRequest called twice for IThing, or
// even mixing RegisterInstance with RegisterHandler for the same service) does not replace the
// earlier registration: GetAllInstances(typeof(IThing), null) - a call neither RegisterSingleton
// nor RegisterPerRequest alone ever needed - comes back with one entry per registration, 2 for 2
// calls, proving every registration is kept, not merely the most recent one.

using System.Linq;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex030_SimpleContainerInstances
{
    /// <summary>Registers a ready-made object as the resolution result for TService - GetInstance will return THAT object, not a new one.</summary>
    public void RegisterInstance<TService>(SimpleContainer container, TService instance) where TService : class =>
        throw new NotImplementedException("TODO: Ex030 - container.RegisterInstance(typeof(TService), null, instance)");

    /// <summary>Registers a FACTORY for TService - every resolution runs the factory afresh and returns whatever it produces.</summary>
    public void RegisterHandler<TService>(SimpleContainer container, Func<SimpleContainer, object> factory) =>
        throw new NotImplementedException("TODO: Ex030 - container.RegisterHandler(typeof(TService), null, factory)");

    /// <summary>How many registrations currently exist for TService - GetAllInstances, not GetInstance, is what surfaces every one, duplicates included.</summary>
    public int CountRegistrations<TService>(SimpleContainer container) =>
        throw new NotImplementedException("TODO: Ex030 - container.GetAllInstances(typeof(TService), null).Count()");
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

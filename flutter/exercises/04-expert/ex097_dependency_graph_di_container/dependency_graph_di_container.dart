// Exercise 097 - a small DI container with scoped lifetimes (expert).
//
// Goal:   Support three registration lifetimes in a DiContainer: singleton
//         (one instance for the container's whole lifetime), transient (a
//         new instance every resolve()), and scoped (one instance per
//         Scope, shared within that scope, distinct across scopes).
// Drills: factory functions, lifetime management, child-scope isolation.
// Passes: when resolve<T>() honors each lifetime — singletons are identical
//         across scopes, transients are always distinct, and scoped
//         instances are identical within one Scope but distinct in another.

enum Lifetime { singleton, transient, scoped }

class DiContainer {
  final Map<Type, (Lifetime, Object Function())> _registrations = {};
  final Map<Type, Object> _singletons = {};

  void register<T extends Object>(
    T Function() factory, {
    required Lifetime lifetime,
  }) {
    _registrations[T] = (lifetime, factory);
  }

  Scope createScope() => Scope(this);

  T resolve<T extends Object>() {
    throw UnimplementedError('TODO');
  }
}

class Scope {
  Scope(this._container);
  final DiContainer _container;
  final Map<Type, Object> _scoped = {};

  T resolve<T extends Object>() {
    throw UnimplementedError('TODO');
  }
}

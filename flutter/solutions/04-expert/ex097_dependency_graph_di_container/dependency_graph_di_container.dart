// Exercise 097 - a small DI container with scoped lifetimes (reference
// solution).

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

  T resolve<T extends Object>() => createScope().resolve<T>();
}

class Scope {
  Scope(this._container);
  final DiContainer _container;
  final Map<Type, Object> _scoped = {};

  T resolve<T extends Object>() {
    final registration = _container._registrations[T];
    if (registration == null) {
      throw StateError('No registration for type $T');
    }
    final (lifetime, factory) = registration;
    switch (lifetime) {
      case Lifetime.transient:
        return factory() as T;
      case Lifetime.singleton:
        return _container._singletons.putIfAbsent(T, factory) as T;
      case Lifetime.scoped:
        return _scoped.putIfAbsent(T, factory) as T;
    }
  }
}

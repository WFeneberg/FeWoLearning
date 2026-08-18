// Exercise 081 - a minimal service locator (advanced).
//
// Goal:   Implement a tiny dependency-injection container, in the style of
//         get_it, that registers and resolves singletons by type.
// Drills: generics, Type as a Map key, service locator pattern.
// Passes: when registerSingleton<T>() stores an instance and get<T>()
//         returns that exact instance, throwing for an unregistered type.

class ServiceLocator {
  final Map<Type, Object> _instances = {};

  void registerSingleton<T extends Object>(T instance) {
    throw UnimplementedError('TODO');
  }

  T get<T extends Object>() {
    throw UnimplementedError('TODO');
  }
}

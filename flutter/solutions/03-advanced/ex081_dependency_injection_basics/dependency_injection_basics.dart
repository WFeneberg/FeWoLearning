// Exercise 081 - a minimal service locator (reference solution).

class ServiceLocator {
  final Map<Type, Object> _instances = {};

  void registerSingleton<T extends Object>(T instance) {
    _instances[T] = instance;
  }

  T get<T extends Object>() {
    final instance = _instances[T];
    if (instance == null) {
      throw StateError('No registered instance for type $T');
    }
    return instance as T;
  }
}

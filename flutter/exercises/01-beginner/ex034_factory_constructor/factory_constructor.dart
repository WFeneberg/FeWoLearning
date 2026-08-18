// Exercise 034 - factory constructors (beginner).
//
// Goal:   Make Logger(name) return the same cached instance for a given
//         name instead of always constructing a new one.
// Drills: factory constructors, caching instances.
// Passes: when Logger('a') called twice returns the identical (`identical`)
//         instance, while Logger('b') returns a different instance.

class Logger {
  final String name;
  static final Map<String, Logger> _cache = {};

  Logger._internal(this.name);

  factory Logger(String name) {
    throw UnimplementedError('TODO');
  }
}

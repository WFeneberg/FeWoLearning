// Exercise 034 - factory constructors (reference solution).

class Logger {
  final String name;
  static final Map<String, Logger> _cache = {};

  Logger._internal(this.name);

  factory Logger(String name) {
    return _cache.putIfAbsent(name, () => Logger._internal(name));
  }
}

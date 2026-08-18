// Exercise 023 - late variables & lazy initialization (beginner).
//
// Goal:   Implement Configuration.initialize() to set a late final field
//         exactly once, and Report._computeTotal() so that Report.total is
//         computed lazily on first access and only once thereafter.
// Drills: late, lazy initialization.
// Passes: when accessing Configuration.environment before initialize()
//         throws, initialize() sets it, and Report.total triggers
//         _computeTotal() only on its first read.

class Configuration {
  late final String environment;

  void initialize(String env) {
    throw UnimplementedError('TODO');
  }
}

int computeCount = 0;

class Report {
  late final int total = _computeTotal();

  int _computeTotal() {
    throw UnimplementedError('TODO');
  }
}

// Exercise 023 - late variables & lazy initialization (reference solution).

class Configuration {
  late final String environment;

  void initialize(String env) {
    environment = env;
  }
}

int computeCount = 0;

class Report {
  late final int total = _computeTotal();

  int _computeTotal() {
    computeCount++;
    return 42;
  }
}

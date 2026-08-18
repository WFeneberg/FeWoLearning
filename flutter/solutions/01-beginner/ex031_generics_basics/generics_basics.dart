// Exercise 031 - generics basics (reference solution).

class Box<T> {
  T? _value;

  void put(T value) {
    _value = value;
  }

  T? take() {
    final current = _value;
    _value = null;
    return current;
  }
}

T firstOrDefault<T>(List<T> items, T fallback) =>
    items.isEmpty ? fallback : items.first;

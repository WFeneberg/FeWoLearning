// Exercise 031 - generics basics (beginner).
//
// Goal:   Implement a single-slot generic Box that can hold and release a
//         value of any type, and a generic helper that returns a list's
//         first element or a fallback when the list is empty.
// Drills: generic classes, generic functions, type parameters.
// Passes: when Box<T>.take() returns and clears whatever was put(), and
//         firstOrDefault() only uses the fallback for an empty list.

class Box<T> {
  T? _value;

  void put(T value) {
    throw UnimplementedError('TODO');
  }

  T? take() {
    throw UnimplementedError('TODO');
  }
}

T firstOrDefault<T>(List<T> items, T fallback) {
  throw UnimplementedError('TODO');
}

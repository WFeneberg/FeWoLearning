// Exercise 045 - generic repository pattern (intermediate).
//
// Goal:   Implement a generic in-memory repository keyed by an id type.
// Drills: generic classes with two type parameters, Map-backed storage.
// Passes: when add()/getById()/getAll() behave like a simple key-value
//         store, with getById() returning null for a missing id.

class InMemoryRepository<Id, T> {
  final Map<Id, T> _items = {};

  void add(Id id, T item) {
    throw UnimplementedError('TODO');
  }

  T? getById(Id id) {
    throw UnimplementedError('TODO');
  }

  List<T> getAll() {
    throw UnimplementedError('TODO');
  }
}

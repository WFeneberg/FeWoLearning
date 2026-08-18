// Exercise 045 - generic repository pattern (reference solution).

class InMemoryRepository<Id, T> {
  final Map<Id, T> _items = {};

  void add(Id id, T item) => _items[id] = item;

  T? getById(Id id) => _items[id];

  List<T> getAll() => _items.values.toList();
}

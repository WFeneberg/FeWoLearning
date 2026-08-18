import 'package:test/test.dart';

import 'generic_repository.dart';

void main() {
  test('add and getById store and retrieve an item', () {
    final repo = InMemoryRepository<int, String>();
    repo.add(1, 'Ada');
    expect(repo.getById(1), 'Ada');
  });

  test('getById returns null for a missing id', () {
    final repo = InMemoryRepository<int, String>();
    expect(repo.getById(99), isNull);
  });

  test('getAll returns every stored item', () {
    final repo = InMemoryRepository<int, String>()
      ..add(1, 'Ada')
      ..add(2, 'Grace');
    expect(repo.getAll(), containsAll(['Ada', 'Grace']));
    expect(repo.getAll().length, 2);
  });
}

import 'package:test/test.dart';

import 'repository_pattern_flutter.dart';

void main() {
  test('findById returns a seeded user', () async {
    final repo = InMemoryUserRepository([const User('1', 'Ada')]);

    final user = await repo.findById('1');

    expect(user?.name, 'Ada');
  });

  test('findById returns null for an unknown id', () async {
    final repo = InMemoryUserRepository([const User('1', 'Ada')]);

    final user = await repo.findById('missing');

    expect(user, isNull);
  });
}

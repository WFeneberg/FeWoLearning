import 'package:test/test.dart';

import 'extension_type.dart';

void main() {
  test('formatted prefixes the id', () {
    expect(UserId(42).formatted(), 'user-42');
  });

  test('extension type exposes the underlying value', () {
    final id = UserId(7);
    expect(id.value, 7);
  });
}

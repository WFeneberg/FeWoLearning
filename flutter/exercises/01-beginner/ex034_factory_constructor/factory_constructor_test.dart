import 'package:test/test.dart';

import 'factory_constructor.dart';

void main() {
  test('Logger returns the identical instance for the same name', () {
    final a1 = Logger('a');
    final a2 = Logger('a');
    expect(identical(a1, a2), isTrue);
  });

  test('Logger returns different instances for different names', () {
    final a = Logger('a');
    final b = Logger('b');
    expect(identical(a, b), isFalse);
  });

  test('cached instance keeps its original name', () {
    final logger = Logger('checkout');
    expect(logger.name, 'checkout');
  });
}

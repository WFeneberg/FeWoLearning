import 'package:test/test.dart';

import 'test_package_assertions.dart';

void main() {
  group('Counter', () {
    late Counter counter;

    setUp(() {
      counter = Counter();
      counter.increment();
      counter.increment();
    });

    tearDown(() {
      counter.reset();
    });

    test('increment increases the value from the setUp baseline', () {
      counter.increment();
      expect(counter.value, 3);
    });

    test('reset sets the value back to zero', () {
      counter.reset();
      expect(counter.value, 0);
    });

    test('each test starts from the setUp baseline of 2', () {
      expect(counter.value, 2);
    });
  });
}

import 'package:test/test.dart';

import 'generics_basics.dart';

void main() {
  test('Box holds and releases a value of any type', () {
    final box = Box<String>();
    box.put('hello');
    expect(box.take(), 'hello');
  });

  test('Box.take clears the stored value', () {
    final box = Box<int>();
    box.put(42);
    box.take();
    expect(box.take(), isNull);
  });

  test('firstOrDefault returns the first element when present', () {
    expect(firstOrDefault([10, 20, 30], -1), 10);
  });

  test('firstOrDefault returns the fallback for an empty list', () {
    expect(firstOrDefault(<int>[], -1), -1);
  });
}

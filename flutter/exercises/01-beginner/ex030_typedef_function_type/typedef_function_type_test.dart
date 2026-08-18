import 'package:test/test.dart';

import 'typedef_function_type.dart';

void main() {
  test('filterBy applies the given predicate', () {
    bool isEven(int v) => v.isEven;
    expect(filterBy([1, 2, 3, 4, 5], isEven), [2, 4]);
  });

  test('greaterThan builds a predicate for strictly-greater values', () {
    final over2 = greaterThan(2);
    expect(filterBy([1, 2, 3, 4, 5], over2), [3, 4, 5]);
  });

  test('greaterThan predicate excludes the threshold itself', () {
    final over3 = greaterThan(3);
    expect(over3(3), isFalse);
    expect(over3(4), isTrue);
  });
}

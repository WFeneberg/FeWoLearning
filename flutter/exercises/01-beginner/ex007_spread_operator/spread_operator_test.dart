import 'package:test/test.dart';

import 'spread_operator.dart';

void main() {
  test('combine concatenates two lists in order', () {
    expect(combine([1, 2], [3, 4]), [1, 2, 3, 4]);
  });

  test('combineWithExtras appends a non-null extras list', () {
    expect(combineWithExtras([1, 2], [3]), [1, 2, 3]);
  });

  test('combineWithExtras ignores a null extras list', () {
    expect(combineWithExtras([1, 2], null), [1, 2]);
  });
}

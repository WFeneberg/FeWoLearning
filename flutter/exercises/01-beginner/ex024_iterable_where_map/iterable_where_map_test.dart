import 'package:test/test.dart';

import 'iterable_where_map.dart';

void main() {
  test('squaresOfEvens keeps only even numbers, squared, in order', () {
    expect(squaresOfEvens([1, 2, 3, 4, 5, 6]), [4, 16, 36]);
  });

  test('squaresOfEvens returns empty for no evens', () {
    expect(squaresOfEvens([1, 3, 5]), <int>[]);
  });

  test('namesStartingWith filters by prefix, preserving order', () {
    expect(namesStartingWith(['Ada', 'Bob', 'Alice', 'Carl'], 'A'),
        ['Ada', 'Alice']);
  });

  test('namesStartingWith returns empty when nothing matches', () {
    expect(namesStartingWith(['Bob', 'Carl'], 'Z'), <String>[]);
  });
}

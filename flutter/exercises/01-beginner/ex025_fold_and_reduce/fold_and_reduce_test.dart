import 'package:test/test.dart';

import 'fold_and_reduce.dart';

void main() {
  test('sumWithFold adds every element', () {
    expect(sumWithFold([1, 2, 3, 4]), 10);
  });

  test('sumWithFold returns 0 for an empty list', () {
    expect(sumWithFold([]), 0);
  });

  test('maxWithReduce finds the largest element', () {
    expect(maxWithReduce([3, 7, 2, 9, 4]), 9);
  });

  test('maxWithReduce throws on an empty list', () {
    expect(() => maxWithReduce([]), throwsStateError);
  });

  test('joinWithFold joins words with a comma and space', () {
    expect(joinWithFold(['a', 'b', 'c']), 'a, b, c');
  });

  test('joinWithFold returns empty string for an empty list', () {
    expect(joinWithFold([]), '');
  });
}

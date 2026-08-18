import 'package:test/test.dart';

import 'arrow_functions.dart';

void main() {
  test('square multiplies a number by itself', () {
    expect(square(5), 25);
  });

  test('isPositiveEven is true only for positive even numbers', () {
    expect(isPositiveEven(4), isTrue);
    expect(isPositiveEven(-4), isFalse);
    expect(isPositiveEven(3), isFalse);
  });

  test('applyTwice applies the function twice in sequence', () {
    expect(applyTwice((x) => x + 1, 5), 7);
  });
}

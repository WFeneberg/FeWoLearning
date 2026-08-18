import 'package:test/test.dart';

import 'string_interpolation.dart';

void main() {
  test('greet interpolates name and age', () {
    expect(greet('Ada', 30), 'Hello, Ada! You are 30 years old.');
  });

  test('receiptLine formats a two-line multiline entry', () {
    expect(receiptLine('Coffee', 3, 2.5), '3x Coffee\n@ 2.50 each');
  });
}

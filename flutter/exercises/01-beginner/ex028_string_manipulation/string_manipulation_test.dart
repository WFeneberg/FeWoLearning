import 'package:test/test.dart';

import 'string_manipulation.dart';

void main() {
  test('splitAndTrim trims whitespace around every field', () {
    expect(splitAndTrim('a, b ,c,  d '), ['a', 'b', 'c', 'd']);
  });

  test('splitAndTrim handles a single field', () {
    expect(splitAndTrim(' solo '), ['solo']);
  });

  test('padId pads short ids with leading zeros', () {
    expect(padId(7), '0007');
  });

  test('padId leaves 4-digit ids unchanged', () {
    expect(padId(1234), '1234');
  });
}

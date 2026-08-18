import 'package:test/test.dart';

import 'pattern_destructuring.dart';

void main() {
  test('formats each (name, score) record', () {
    expect(
      formatScores([('Ada', 100), ('Grace', 95)]),
      ['Ada: 100', 'Grace: 95'],
    );
  });

  test('empty list yields an empty list', () {
    expect(formatScores(<(String, int)>[]), <String>[]);
  });
}

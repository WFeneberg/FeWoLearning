import 'package:test/test.dart';

import 'cascade_notation.dart';

void main() {
  test('buildReport underlines the title and lists each line', () {
    expect(
      buildReport('Report', ['a', 'b']),
      'Report\n------\na\nb\n',
    );
  });

  test('buildReport underlines with a length matching the title', () {
    expect(buildReport('Hi', []), 'Hi\n--\n');
  });
}

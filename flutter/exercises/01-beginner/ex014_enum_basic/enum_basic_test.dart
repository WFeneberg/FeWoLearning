import 'package:test/test.dart';

import 'enum_basic.dart';

void main() {
  test('opposite of north is south', () {
    expect(opposite(Direction.north), Direction.south);
  });

  test('opposite of east is west', () {
    expect(opposite(Direction.east), Direction.west);
  });

  test('opposite is its own inverse', () {
    for (final d in Direction.values) {
      expect(opposite(opposite(d)), d);
    }
  });
}

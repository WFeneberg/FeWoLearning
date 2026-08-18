import 'package:test/test.dart';

import 'switch_pattern.dart';

void main() {
  test('origin is (0, 0)', () {
    expect(classify((0, 0)), 'origin');
  });

  test('points on the x-axis are on-axis', () {
    expect(classify((5, 0)), 'on-axis');
  });

  test('points on the y-axis are on-axis', () {
    expect(classify((0, -3)), 'on-axis');
  });

  test('points off both axes are quadrant', () {
    expect(classify((2, 3)), 'quadrant');
  });
}

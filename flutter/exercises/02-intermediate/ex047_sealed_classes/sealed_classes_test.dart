import 'package:test/test.dart';

import 'sealed_classes.dart';

void main() {
  test('area of a circle', () {
    expect(area(Circle(2)), closeTo(12.566, 0.001));
  });

  test('area of a rectangle', () {
    expect(area(Rectangle(3, 4)), 12);
  });
}

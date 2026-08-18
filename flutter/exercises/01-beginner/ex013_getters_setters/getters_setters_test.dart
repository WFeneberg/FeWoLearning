import 'package:test/test.dart';

import 'getters_setters.dart';

void main() {
  test('area is pi times radius squared', () {
    expect(Circle(2).area, closeTo(12.566, 0.001));
  });

  test('diameter is twice the radius', () {
    expect(Circle(3).diameter, 6);
  });

  test('setting diameter updates the radius', () {
    final circle = Circle(2);
    circle.diameter = 10;
    expect(circle.radius, 5);
  });
}

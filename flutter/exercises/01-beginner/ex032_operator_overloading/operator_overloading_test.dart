import 'package:test/test.dart';

import 'operator_overloading.dart';

void main() {
  test('+ adds components', () {
    final v = const Vector2(1, 2) + const Vector2(3, 4);
    expect(v.x, 4);
    expect(v.y, 6);
  });

  test('- subtracts components', () {
    final v = const Vector2(5, 5) - const Vector2(2, 3);
    expect(v.x, 3);
    expect(v.y, 2);
  });

  test('unary - negates components', () {
    final v = -const Vector2(1, -2);
    expect(v.x, -1);
    expect(v.y, 2);
  });

  test('* scales components', () {
    final v = const Vector2(2, 3) * 2.5;
    expect(v.x, 5.0);
    expect(v.y, 7.5);
  });
}

import 'package:test/test.dart';

import 'enhanced_enum.dart';

void main() {
  test('each planet exposes its gravity', () {
    expect(Planet.mercury.gravity, 3.7);
    expect(Planet.earth.gravity, 9.8);
  });

  test('surfaceWeight multiplies mass by gravity', () {
    expect(Planet.earth.surfaceWeight(10), closeTo(98.0, 0.0001));
  });

  test('the same mass weighs less on Mercury than on Jupiter', () {
    expect(
      Planet.mercury.surfaceWeight(10),
      lessThan(Planet.jupiter.surfaceWeight(10)),
    );
  });
}

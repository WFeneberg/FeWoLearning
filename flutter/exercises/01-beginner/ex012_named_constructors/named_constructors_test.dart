import 'package:test/test.dart';

import 'named_constructors.dart';

void main() {
  test('Temperature.celsius stores the value directly', () {
    expect(Temperature.celsius(100).celsius, 100);
  });

  test('Temperature.fahrenheit converts freezing point to 0', () {
    expect(Temperature.fahrenheit(32).celsius, closeTo(0, 0.0001));
  });

  test('Temperature.fahrenheit converts boiling point to 100', () {
    expect(Temperature.fahrenheit(212).celsius, closeTo(100, 0.0001));
  });
}

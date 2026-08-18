import 'package:test/test.dart';

import 'mixin_basics.dart';

void main() {
  test("fly reports the flyer's speed", () {
    expect(Bird(50).fly(), 'Flying at 50.0 km/h');
  });

  test('different flyers report their own speed', () {
    expect(Bird(120).fly(), 'Flying at 120.0 km/h');
  });
}

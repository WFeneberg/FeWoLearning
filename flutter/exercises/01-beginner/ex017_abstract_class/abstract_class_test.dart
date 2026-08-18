import 'package:test/test.dart';

import 'abstract_class.dart';

void main() {
  test("describe reports the shape's area", () {
    expect(Square(4).describe(), 'Area: 16.0');
  });

  test("describe uses the subclass's own area()", () {
    expect(Square(2).describe(), 'Area: 4.0');
  });
}

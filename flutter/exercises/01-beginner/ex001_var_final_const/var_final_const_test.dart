import 'package:test/test.dart';

import 'var_final_const.dart';

void main() {
  test('area multiplies width and height', () {
    expect(area(4, 5), 20);
  });

  test('perimeter sums all four sides', () {
    expect(perimeter(4, 5), 18);
  });

  test('priceWithTax applies the tax rate constant', () {
    expect(priceWithTax(100), closeTo(119.0, 0.0001));
  });
}

import 'package:test/test.dart';

import 'default_param_values.dart';

void main() {
  test('repeatJoin defaults to a single copy', () {
    expect(repeatJoin('hi'), 'hi');
  });

  test('repeatJoin repeats with the default separator', () {
    expect(repeatJoin('hi', 3), 'hi, hi, hi');
  });

  test('repeatJoin honors a custom separator', () {
    expect(repeatJoin('hi', 3, ' - '), 'hi - hi - hi');
  });
}

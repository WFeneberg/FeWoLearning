import 'package:test/test.dart';

import 'set_operations.dart';

void main() {
  final a = {1, 2, 3, 4};
  final b = {3, 4, 5, 6};

  test('commonElements returns the intersection', () {
    expect(commonElements(a, b), {3, 4});
  });

  test('onlyInFirst returns the difference', () {
    expect(onlyInFirst(a, b), {1, 2});
  });

  test('allElements returns the union', () {
    expect(allElements(a, b), {1, 2, 3, 4, 5, 6});
  });

  test('commonElements is empty for disjoint sets', () {
    expect(commonElements({1, 2}, {3, 4}), <int>{});
  });
}

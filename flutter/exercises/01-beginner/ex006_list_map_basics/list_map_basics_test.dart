import 'package:test/test.dart';

import 'list_map_basics.dart';

void main() {
  test('secondItem returns the item at index 1', () {
    expect(secondItem(['apple', 'banana', 'cherry']), 'banana');
  });

  test('priceFor returns the price for a known item', () {
    expect(priceFor({'apple': 2, 'banana': 1}, 'apple'), 2);
  });

  test('priceFor returns 0 for an unknown item', () {
    expect(priceFor({'apple': 2, 'banana': 1}, 'cherry'), 0);
  });
}

import 'package:test/test.dart';

import 'named_positional_params.dart';

void main() {
  test('formatAddress defaults country to Germany', () {
    expect(
      formatAddress('Main St 1', 'Berlin', postalCode: '10115'),
      'Main St 1, 10115 Berlin, Germany',
    );
  });

  test('formatAddress accepts an explicit country', () {
    expect(
      formatAddress('5th Ave', 'New York', postalCode: '10001', country: 'USA'),
      '5th Ave, 10001 New York, USA',
    );
  });
}

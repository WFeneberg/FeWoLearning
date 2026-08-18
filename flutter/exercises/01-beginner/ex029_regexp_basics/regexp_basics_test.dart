import 'package:test/test.dart';

import 'regexp_basics.dart';

void main() {
  test('isValidEmail accepts a plausible address', () {
    expect(isValidEmail('ada@example.com'), isTrue);
  });

  test('isValidEmail rejects a missing @', () {
    expect(isValidEmail('ada.example.com'), isFalse);
  });

  test('isValidEmail rejects a missing domain suffix', () {
    expect(isValidEmail('ada@example'), isFalse);
  });

  test('maskDigits replaces every digit with #', () {
    expect(maskDigits('Order #4711, x2'), 'Order #####, x#');
  });

  test('maskDigits leaves non-digits untouched', () {
    expect(maskDigits('no digits here'), 'no digits here');
  });
}

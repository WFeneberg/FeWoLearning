import 'package:test/test.dart';

import 'null_safety_basics.dart';

void main() {
  test('displayName prefers a present nickname', () {
    expect(displayName('Ace', 'Alice Cooper'), 'Ace');
  });

  test('displayName falls back to the full name when nickname is null', () {
    expect(displayName(null, 'Alice Cooper'), 'Alice Cooper');
  });

  test('applyDefaultTheme keeps an existing theme', () {
    expect(applyDefaultTheme('dark'), 'dark');
  });

  test('applyDefaultTheme defaults to light when null', () {
    expect(applyDefaultTheme(null), 'light');
  });
}

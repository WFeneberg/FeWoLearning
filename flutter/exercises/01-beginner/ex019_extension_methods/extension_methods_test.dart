import 'package:test/test.dart';

import 'extension_methods.dart';

void main() {
  test('isPalindrome is true for a palindrome', () {
    expect('level'.isPalindrome, isTrue);
  });

  test('isPalindrome ignores case', () {
    expect('Racecar'.isPalindrome, isTrue);
  });

  test('isPalindrome is false for a non-palindrome', () {
    expect('hello'.isPalindrome, isFalse);
  });

  test('shout upper-cases and adds an exclamation mark', () {
    expect('hi'.shout(), 'HI!');
  });
}

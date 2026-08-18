// Exercise 019 - extension methods (beginner).
//
// Goal:   Add palindrome-checking and a shout-formatter directly onto
//         String, without subclassing or wrapping it.
// Drills: extension methods, extension getters.
// Passes: when isPalindrome ignores case and shout() upper-cases the
//         string with a trailing "!".

extension StringX on String {
  bool get isPalindrome {
    throw UnimplementedError('TODO');
  }

  String shout() {
    throw UnimplementedError('TODO');
  }
}

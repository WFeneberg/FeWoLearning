// Exercise 019 - extension methods (reference solution).

extension StringX on String {
  bool get isPalindrome {
    final lower = toLowerCase();
    return lower == lower.split('').reversed.join();
  }

  String shout() => '${toUpperCase()}!';
}

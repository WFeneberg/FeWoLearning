// Exercise 029 - RegExp basics (reference solution).

final _emailPattern = RegExp(r'^[\w.+-]+@[\w-]+\.[a-zA-Z]{2,}$');

bool isValidEmail(String input) => _emailPattern.hasMatch(input);

String maskDigits(String input) => input.replaceAll(RegExp(r'\d'), '#');

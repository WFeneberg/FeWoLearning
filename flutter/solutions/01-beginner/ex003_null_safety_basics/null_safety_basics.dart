// Exercise 003 - null safety basics (reference solution).

String displayName(String? nickname, String fullName) => nickname ?? fullName;

String applyDefaultTheme(String? currentTheme) {
  currentTheme ??= 'light';
  return currentTheme;
}

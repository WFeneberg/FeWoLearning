// Exercise 037 - async/await basics (reference solution).

Future<int> fetchDouble(int value) async {
  await Future<void>.delayed(const Duration(milliseconds: 1));
  return value * 2;
}

Future<int> fetchSquareOfDouble(int value) async {
  final doubled = await fetchDouble(value);
  return doubled * doubled;
}

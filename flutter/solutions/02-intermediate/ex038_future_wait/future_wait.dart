// Exercise 038 - Future.wait (reference solution).

Future<int> sumInParallel(List<int> values) async {
  final doubled = await Future.wait(values.map(_doubleAsync));
  return doubled.fold(0, (acc, v) => acc + v);
}

Future<int> _doubleAsync(int value) async {
  await Future<void>.delayed(const Duration(milliseconds: 1));
  return value * 2;
}

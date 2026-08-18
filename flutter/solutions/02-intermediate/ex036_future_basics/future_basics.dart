// Exercise 036 - Future basics (reference solution).

Future<int> doubleAsync(int value) {
  if (value < 0) {
    return Future.error(ArgumentError('value must not be negative'));
  }
  return Future.value(value).then((v) => v * 2);
}

Future<int> doubleWithFallback(Future<int> source, int fallback) {
  return source.then((v) => v * 2).catchError((_) => fallback);
}

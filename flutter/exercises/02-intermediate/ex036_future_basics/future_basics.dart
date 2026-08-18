// Exercise 036 - Future basics (intermediate).
//
// Goal:   Double a value asynchronously, and separately chain a doubling
//         step onto an existing Future with a fallback if it fails.
// Drills: Future, Future.value, Future.error, .then, .catchError.
// Passes: when doubleAsync() rejects negative input and doubleWithFallback()
//         recovers to the fallback value on failure.

Future<int> doubleAsync(int value) {
  throw UnimplementedError('TODO');
}

Future<int> doubleWithFallback(Future<int> source, int fallback) {
  throw UnimplementedError('TODO');
}

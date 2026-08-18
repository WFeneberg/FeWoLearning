// Exercise 039 - Stream basics (intermediate).
//
// Goal:   Build a stream that counts up to n using an async* generator,
//         and a helper that collects a stream's values into a list.
// Drills: Stream, async*, yield, await for.
// Passes: when countUpTo() emits 1..n in order and collect() gathers every
//         emitted value.

Stream<int> countUpTo(int n) async* {
  throw UnimplementedError('TODO');
}

Future<List<int>> collect(Stream<int> stream) {
  throw UnimplementedError('TODO');
}

// Exercise 039 - Stream basics (reference solution).

Stream<int> countUpTo(int n) async* {
  for (var i = 1; i <= n; i++) {
    yield i;
  }
}

Future<List<int>> collect(Stream<int> stream) => stream.toList();

// Exercise 049 - async* generators (intermediate).
//
// Goal:   Split a list into pages of at most pageSize items, yielding one
//         page at a time.
// Drills: async*, yield, Stream generators.
// Passes: when paginate() yields consecutive slices of at most pageSize
//         items, with the last page possibly shorter.

Stream<List<int>> paginate(List<int> items, int pageSize) async* {
  throw UnimplementedError('TODO');
}

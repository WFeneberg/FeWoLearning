// Exercise 037 - async/await basics (intermediate).
//
// Goal:   Await one async step, then feed its result into a second async
//         step, without using .then chains.
// Drills: async/await, sequential futures.
// Passes: when fetchSquareOfDouble() awaits fetchDouble() before squaring
//         its result.

Future<int> fetchDouble(int value) async {
  throw UnimplementedError('TODO');
}

Future<int> fetchSquareOfDouble(int value) async {
  throw UnimplementedError('TODO');
}

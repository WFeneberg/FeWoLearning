// Exercise 042 - Isolate.run / compute-style offloading (intermediate).
//
// Goal:   Compute the sum of squares from 1..n on a background isolate.
// Drills: dart:isolate, Isolate.run, offloading CPU-bound work.
// Passes: when sumOfSquaresInIsolate() runs the computation via
//         Isolate.run() rather than on the calling isolate.

Future<int> sumOfSquaresInIsolate(int n) {
  throw UnimplementedError('TODO');
}

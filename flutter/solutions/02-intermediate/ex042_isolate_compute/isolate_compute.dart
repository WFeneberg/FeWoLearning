// Exercise 042 - Isolate.run / compute-style offloading (reference solution).

import 'dart:isolate';

Future<int> sumOfSquaresInIsolate(int n) => Isolate.run(() => _sumOfSquares(n));

int _sumOfSquares(int n) {
  var total = 0;
  for (var i = 1; i <= n; i++) {
    total += i * i;
  }
  return total;
}

import 'package:test/test.dart';

import 'isolate_compute.dart';

void main() {
  test('sumOfSquaresInIsolate computes the sum of squares up to n', () async {
    expect(await sumOfSquaresInIsolate(3), 14); // 1 + 4 + 9
  });

  test('sumOfSquaresInIsolate(0) is 0', () async {
    expect(await sumOfSquaresInIsolate(0), 0);
  });
}

import 'package:test/test.dart';

import 'future_wait.dart';

void main() {
  test('sumInParallel doubles each value and sums them', () async {
    expect(await sumInParallel([1, 2, 3]), 12);
  });

  test('sumInParallel handles an empty list', () async {
    expect(await sumInParallel(<int>[]), 0);
  });

  test('sumInParallel handles a single value', () async {
    expect(await sumInParallel([5]), 10);
  });
}

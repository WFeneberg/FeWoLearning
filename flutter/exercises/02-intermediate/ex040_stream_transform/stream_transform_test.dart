import 'package:test/test.dart';

import 'stream_transform.dart';

void main() {
  test('evenSquares keeps only even numbers, squared', () async {
    final result =
        await evenSquares(Stream.fromIterable([1, 2, 3, 4, 5, 6])).toList();
    expect(result, [4, 16, 36]);
  });

  test('evenSquares on an all-odd stream yields nothing', () async {
    final result = await evenSquares(Stream.fromIterable([1, 3, 5])).toList();
    expect(result, <int>[]);
  });
}

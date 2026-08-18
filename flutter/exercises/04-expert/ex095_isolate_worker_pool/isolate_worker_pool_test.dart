import 'dart:io';

import 'package:test/test.dart';

import 'isolate_worker_pool.dart';

List<int> _timedJob(int delayMs) {
  final start = DateTime.now().millisecondsSinceEpoch;
  sleep(Duration(milliseconds: delayMs));
  final end = DateTime.now().millisecondsSinceEpoch;
  return [start, end];
}

int _maxOverlap(List<List<int>> intervals) {
  final events = <List<int>>[];
  for (final interval in intervals) {
    events.add([interval[0], 1]);
    events.add([interval[1], -1]);
  }
  events.sort((a, b) => a[0] != b[0] ? a[0] - b[0] : a[1] - b[1]);
  var current = 0;
  var maxSeen = 0;
  for (final event in events) {
    current += event[1];
    if (current > maxSeen) maxSeen = current;
  }
  return maxSeen;
}

void main() {
  test('run executes every job and preserves submission order', () async {
    final pool = WorkerPool(4);
    final results = await pool.run<int, int>([1, 2, 3], (n) => n * n);
    expect(results, [1, 4, 9]);
  });

  test(
    'run never exceeds maxConcurrency jobs running at once',
    () async {
      final pool = WorkerPool(2);
      final intervals = await pool.run<int, List<int>>(
        List.filled(6, 60),
        _timedJob,
      );
      expect(_maxOverlap(intervals), lessThanOrEqualTo(2));
    },
    timeout: const Timeout(Duration(seconds: 10)),
  );

  test('run with an empty job list returns an empty list', () async {
    final pool = WorkerPool(3);
    final results = await pool.run<int, int>([], (n) => n);
    expect(results, isEmpty);
  });
}

// Exercise 074 - combining latest stream values (advanced).
//
// Goal:   Combine two streams into one that emits a combined value every
//         time either source emits, once both have emitted at least once —
//         in the style of rxdart's combineLatest.
// Drills: StreamController, broadcast fan-in, manual multi-stream merging.
// Passes: when combineLatest2() emits nothing until both sources have
//         emitted once, then re-emits on every subsequent event from either.

import 'dart:async';

Stream<R> combineLatest2<A, B, R>(
  Stream<A> a,
  Stream<B> b,
  R Function(A, B) combiner,
) {
  throw UnimplementedError('TODO');
}

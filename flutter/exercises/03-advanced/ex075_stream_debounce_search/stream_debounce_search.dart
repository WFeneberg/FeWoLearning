// Exercise 075 - debounced search stream (advanced).
//
// Goal:   Debounce a stream of search-query events so only the last query
//         within a quiet window is emitted — in the style of rxdart's
//         debounce.
// Drills: Timer, StreamController, cancelling superseded pending work.
// Passes: when debounce() suppresses events superseded within the window
//         and still emits the final value once the source goes quiet.

import 'dart:async';

Stream<T> debounce<T>(Stream<T> source, Duration duration) {
  throw UnimplementedError('TODO');
}

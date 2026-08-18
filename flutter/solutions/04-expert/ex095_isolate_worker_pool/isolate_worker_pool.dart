// Exercise 095 - isolate worker pool with bounded concurrency (reference
// solution).

import 'dart:async';
import 'dart:isolate';

class WorkerPool {
  WorkerPool(this.maxConcurrency) : assert(maxConcurrency > 0);

  final int maxConcurrency;

  Future<List<R>> run<T, R>(List<T> jobs, R Function(T) work) async {
    final results = List<R?>.filled(jobs.length, null);
    var nextIndex = 0;
    var active = 0;
    final completer = Completer<void>();

    void maybeComplete() {
      if (nextIndex >= jobs.length && active == 0 && !completer.isCompleted) {
        completer.complete();
      }
    }

    void startNext() {
      while (active < maxConcurrency && nextIndex < jobs.length) {
        final index = nextIndex++;
        active++;
        Isolate.run(() => work(jobs[index])).then((result) {
          results[index] = result;
          active--;
          startNext();
          maybeComplete();
        });
      }
      maybeComplete();
    }

    startNext();
    await completer.future;
    return results.cast<R>();
  }
}

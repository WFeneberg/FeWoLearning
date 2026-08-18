// Exercise 095 - isolate worker pool with bounded concurrency (expert).
//
// Goal:   Run CPU-bound jobs on separate isolates (via Isolate.run) while
//         never letting more than maxConcurrency jobs execute at once.
// Drills: dart:isolate (Isolate.run), bounded concurrency without a real
//         semaphore package, async queuing.
// Passes: when run() executes every submitted job on its own isolate,
//         returns results in submission order, and never exceeds
//         maxConcurrency jobs running concurrently.

class WorkerPool {
  WorkerPool(this.maxConcurrency) : assert(maxConcurrency > 0);

  final int maxConcurrency;

  Future<List<R>> run<T, R>(List<T> jobs, R Function(T) work) async {
    throw UnimplementedError('TODO');
  }
}

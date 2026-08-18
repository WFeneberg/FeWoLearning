// Exercise 030 - typedef & function types (beginner).
//
// Goal:   Filter a list of numbers by an injected predicate, and build a
//         reusable "greater than" predicate via a factory function.
// Drills: typedef, function types as first-class values.
// Passes: when filterBy() applies whatever IntPredicate it's given, and
//         greaterThan(threshold) returns a predicate matching values
//         strictly greater than threshold.

typedef IntPredicate = bool Function(int value);

List<int> filterBy(List<int> numbers, IntPredicate predicate) {
  throw UnimplementedError('TODO');
}

IntPredicate greaterThan(int threshold) {
  throw UnimplementedError('TODO');
}

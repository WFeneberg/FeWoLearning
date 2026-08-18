// Exercise 030 - typedef & function types (reference solution).

typedef IntPredicate = bool Function(int value);

List<int> filterBy(List<int> numbers, IntPredicate predicate) =>
    numbers.where(predicate).toList();

IntPredicate greaterThan(int threshold) => (value) => value > threshold;

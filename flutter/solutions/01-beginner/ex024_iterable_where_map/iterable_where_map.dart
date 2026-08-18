// Exercise 024 - Iterable.where/map (reference solution).

List<int> squaresOfEvens(List<int> numbers) =>
    numbers.where((n) => n.isEven).map((n) => n * n).toList();

List<String> namesStartingWith(List<String> names, String prefix) =>
    names.where((n) => n.startsWith(prefix)).toList();

// Exercise 040 - Stream.map/where (reference solution).

Stream<int> evenSquares(Stream<int> source) =>
    source.where((n) => n.isEven).map((n) => n * n);

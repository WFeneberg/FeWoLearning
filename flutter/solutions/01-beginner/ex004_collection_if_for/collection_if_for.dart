// Exercise 004 - collection-for / collection-if (reference solution).

List<int> evensBelow(int n) => [
      for (var i = 0; i < n; i++)
        if (i.isEven) i,
    ];

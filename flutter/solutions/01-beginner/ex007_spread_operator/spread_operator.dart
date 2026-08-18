// Exercise 007 - spread operator (reference solution).

List<int> combine(List<int> a, List<int> b) => [...a, ...b];

List<int> combineWithExtras(List<int> base, List<int>? extras) => [
      ...base,
      ...?extras,
    ];

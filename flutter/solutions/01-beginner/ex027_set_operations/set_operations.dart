// Exercise 027 - Set operations (reference solution).

Set<int> commonElements(Set<int> a, Set<int> b) => a.intersection(b);

Set<int> onlyInFirst(Set<int> a, Set<int> b) => a.difference(b);

Set<int> allElements(Set<int> a, Set<int> b) => a.union(b);

// Exercise 010 - arrow functions (reference solution).

int square(int x) => x * x;

bool isPositiveEven(int x) => x > 0 && x.isEven;

int applyTwice(int Function(int) f, int x) => f(f(x));

// Reference solution — exercise 051.
export function mapEach<T, U>(items: readonly T[], fn: (item: T) => U): U[] {
  return items.map((item) => fn(item));
}

// B is inferred from `first`'s return type and then constrains `second`'s
// parameter, so the two are checked against each other with nothing
// written at the call site.
export function pipe2<A, B, C>(
  first: (input: A) => B,
  second: (middle: B) => C,
): (input: A) => C {
  return (input) => second(first(input));
}

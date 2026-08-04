import { filter, map, Observable, OperatorFunction, scan, tap } from "rxjs";

// Exercise 049 — map, filter and the pipe operator (reference solution).

export function double(): OperatorFunction<number, number> {
  // An operator is just a value, so it can be named, reused and tested on its own.
  return map((value) => value * 2);
}

export function evensOnly(): OperatorFunction<number, number> {
  return filter((value) => value % 2 === 0);
}

export function asMoney(): OperatorFunction<number, string> {
  return map((value) => value.toFixed(2));
}

export function runningTotal(): OperatorFunction<number, number> {
  // scan, not reduce: one emission per input, rather than one at completion.
  return scan((total, value) => total + value, 0);
}

export function countInto(counter: { count: number }): OperatorFunction<number, number> {
  // tap is for the things that are not transformations. The value passes through untouched —
  // a tap that changed it would be a map in disguise, and a reader would not expect it.
  return tap(() => (counter.count += 1));
}

export function moneyForEvens(source: Observable<number>): Observable<string> {
  return source.pipe(evensOnly(), double(), asMoney());
}

export function moneyForEvensImperative(source: Observable<number>): string[] {
  const results: string[] = [];
  // Deliberately bad. It happens to work for a synchronous source and returns nothing useful
  // for any other kind, because there is no way to wait for a stream from a plain function.
  source.subscribe((value) => {
    if (value % 2 === 0) {
      results.push((value * 2).toFixed(2));
    }
  });
  return results;
}

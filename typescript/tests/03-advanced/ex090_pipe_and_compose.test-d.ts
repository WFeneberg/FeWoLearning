import { expectTypeOf, test } from "vitest";
import { compose, pipe } from "@ex/03-advanced/ex090_pipe_and_compose/index";

const double = (n: number): number => n * 2;
const toText = (n: number): string => `n=${n}`;
const lengthOf = (s: string): number => s.length;

// Nothing is annotated at any call site: each overload infers the whole
// chain end to end.
test("pipe reports the end-to-end type at each arity", () => {
  expectTypeOf(pipe(double)).toEqualTypeOf<(a: number) => number>();
  expectTypeOf(pipe(double, toText)).toEqualTypeOf<(a: number) => string>();
  expectTypeOf(pipe(double, toText, lengthOf)).toEqualTypeOf<(a: number) => number>();
});

test("compose reports it the other way round", () => {
  expectTypeOf(compose(toText, double)).toEqualTypeOf<(a: number) => string>();
  expectTypeOf(compose(lengthOf, toText, double)).toEqualTypeOf<(a: number) => number>();
});

// Kept on one line: @ts-expect-error suppresses only the next line, and
// a mismatch is reported at an argument rather than at the call (ex051).
test("pipe rejects a link the previous one cannot feed", () => {
  // @ts-expect-error — double returns a number, lengthOf wants a string
  pipe(double, lengthOf);
});

// Note the order. compose(double, lengthOf) is double(lengthOf(x)) and
// is perfectly valid; it is the OTHER arrangement that does not chain.
test("compose rejects one too, in its own direction", () => {
  // @ts-expect-error — double runs first and returns a number, lengthOf wants a string
  compose(lengthOf, double);
});

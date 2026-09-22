import { expectTypeOf, test } from "vitest";
import { mapEach, pipe2 } from "@ex/02-intermediate/ex051_inference_sites/index";

// The callbacks below carry NO annotations. That only compiles because the
// parameter is contextually typed from the signature it is passed to —
// which is the row.
test("mapEach infers the element type and the result type", () => {
  expectTypeOf(mapEach([1, 2, 3], (n) => n * 2)).toEqualTypeOf<number[]>();
});

test("mapEach lets the callback change the type", () => {
  expectTypeOf(mapEach(["a", "bb"], (s) => s.length)).toEqualTypeOf<number[]>();
});

// B flows out of `first` and into `second`: the second callback's `s` is
// a string because the first returns one, with nothing written down.
test("pipe2 types the second function from the first's return", () => {
  expectTypeOf(
    pipe2(
      (n: number) => String(n),
      (s) => s.length,
    ),
  ).toEqualTypeOf<(input: number) => number>();
});

// Kept on ONE line deliberately. @ts-expect-error suppresses only the
// line immediately after it, and measured, the compiler reports this
// mismatch at the FIRST argument — so a call broken across lines leaves
// the directive unused and the error unsuppressed.
test("pipe2 rejects a second function the first cannot feed", () => {
  // @ts-expect-error — first returns a string, second wants a number
  pipe2((n: number) => String(n), (m: number) => m + 1);
});

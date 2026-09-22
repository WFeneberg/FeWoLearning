import { expectTypeOf, test } from "vitest";
import type {
  Callback,
  UndefinedCallback,
} from "@ex/01-beginner/ex008_function_types/index";

test("Callback returns void", () => {
  expectTypeOf<Callback>().toEqualTypeOf<(line: string) => void>();
});

test("UndefinedCallback returns undefined", () => {
  expectTypeOf<UndefinedCallback>().toEqualTypeOf<(line: string) => undefined>();
});

type IsAssignable<A, B> = A extends B ? true : false;

// Asserted as a pair rather than as two facts: while both types are still
// `unknown`, EVERYTHING is assignable to them, so the positive half alone
// would be green on the untouched stub and would grade nothing.
test("void accepts a value-returning function and undefined does not", () => {
  expectTypeOf<
    [IsAssignable<() => number, Callback>, IsAssignable<() => number, UndefinedCallback>]
  >().toEqualTypeOf<[true, false]>();
});

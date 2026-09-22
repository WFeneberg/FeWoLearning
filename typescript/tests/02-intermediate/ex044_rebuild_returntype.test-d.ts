import { expectTypeOf, test } from "vitest";
import type {
  AsyncReturnType,
  MyParameters,
  MyReturnType,
} from "@ex/02-intermediate/ex044_rebuild_returntype/index";

test("MyReturnType reads the return type", () => {
  expectTypeOf<MyReturnType<() => string>>().toEqualTypeOf<string>();
  expectTypeOf<MyReturnType<(a: number) => boolean>>().toEqualTypeOf<boolean>();
});

test("MyReturnType gives never for a non-function", () => {
  expectTypeOf<MyReturnType<string>>().toEqualTypeOf<never>();
});

// A tuple, not a union: one type per position, which is what makes
// Parameters worth having at all.
test("MyParameters keeps one type per position", () => {
  expectTypeOf<MyParameters<(a: string, b: number) => void>>().toEqualTypeOf<
    [a: string, b: number]
  >();
});

test("MyParameters of a nullary function is the empty tuple", () => {
  expectTypeOf<MyParameters<() => void>>().toEqualTypeOf<[]>();
});

test("MyParameters carries a rest element through", () => {
  expectTypeOf<MyParameters<(first: string, ...rest: number[]) => void>>().toEqualTypeOf<
    [first: string, ...rest: number[]]
  >();
});

// The one the standard library does not provide.
test("AsyncReturnType unwraps the promise", () => {
  expectTypeOf<AsyncReturnType<() => Promise<string>>>().toEqualTypeOf<string>();
});

test("AsyncReturnType leaves a synchronous return alone", () => {
  expectTypeOf<AsyncReturnType<() => number>>().toEqualTypeOf<number>();
});

test("AsyncReturnType gives never for a non-function", () => {
  expectTypeOf<AsyncReturnType<Promise<string>>>().toEqualTypeOf<never>();
});

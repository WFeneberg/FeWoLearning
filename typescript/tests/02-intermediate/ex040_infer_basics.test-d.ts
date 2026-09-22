import { expectTypeOf, test } from "vitest";
import type {
  ElementOf,
  FirstParam,
  Resolved,
  Swapped,
} from "@ex/02-intermediate/ex040_infer_basics/index";

test("ElementOf reads the element type out of an array", () => {
  expectTypeOf<ElementOf<string[]>>().toEqualTypeOf<string>();
  expectTypeOf<ElementOf<readonly number[]>>().toEqualTypeOf<number>();
});

test("ElementOf gives never for a non-array", () => {
  expectTypeOf<ElementOf<number>>().toEqualTypeOf<never>();
});

test("Resolved unwraps a promise", () => {
  expectTypeOf<Resolved<Promise<string>>>().toEqualTypeOf<string>();
});

// Exactly one layer. A recursive version is ex070's job.
test("Resolved unwraps only one layer", () => {
  expectTypeOf<Resolved<Promise<Promise<string>>>>().toEqualTypeOf<Promise<string>>();
});

test("Resolved passes a non-promise through", () => {
  expectTypeOf<Resolved<number>>().toEqualTypeOf<number>();
});

test("FirstParam reads the first parameter of any arity", () => {
  expectTypeOf<FirstParam<(a: string, b: number) => void>>().toEqualTypeOf<string>();
  expectTypeOf<FirstParam<(a: boolean) => void>>().toEqualTypeOf<boolean>();
});

test("FirstParam gives never when there is no first parameter", () => {
  expectTypeOf<FirstParam<() => void>>().toEqualTypeOf<never>();
  expectTypeOf<FirstParam<string>>().toEqualTypeOf<never>();
});

test("Swapped reverses a pair", () => {
  expectTypeOf<Swapped<[string, number]>>().toEqualTypeOf<[number, string]>();
});

test("Swapped gives never for anything that is not a pair", () => {
  expectTypeOf<Swapped<[string]>>().toEqualTypeOf<never>();
  expectTypeOf<Swapped<[string, number, boolean]>>().toEqualTypeOf<never>();
});

import { expectTypeOf, test } from "vitest";
import type {
  Coordinate,
  NonEmptyStrings,
} from "@ex/01-beginner/ex006_tuple_basics/index";

// The labels below are for the reader. Measured: an unlabelled
// [number, number] passes this fact too, because tuple element labels are
// erased for assignability — so this grades arity and element types only.
test("Coordinate is a pair of numbers", () => {
  expectTypeOf<Coordinate>().toEqualTypeOf<[latitude: number, longitude: number]>();
});

// A fixed tuple has a literal length; number[] has length: number. This is
// the fact that separates a tuple from an array of the same element type.
test("Coordinate has exactly two positions", () => {
  expectTypeOf<Coordinate["length"]>().toEqualTypeOf<2>();
});

test("NonEmptyStrings requires a head and allows a tail", () => {
  expectTypeOf<NonEmptyStrings>().toEqualTypeOf<[string, ...string[]]>();
});

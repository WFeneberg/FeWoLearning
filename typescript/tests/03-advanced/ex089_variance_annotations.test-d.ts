import { expectTypeOf, test } from "vitest";
import type {
  Animal,
  Box,
  Consumer,
  Dog,
  Producer,
} from "@ex/03-advanced/ex089_variance_annotations/index";

type Assignable<A, B> = A extends B ? true : false;

// Both directions in one fact each, because a stub of `unknown`-shaped
// members accepts everything and a one-sided fact would be green.
test("a producer is covariant: a Dog producer is an Animal producer", () => {
  expectTypeOf<
    [Assignable<Producer<Dog>, Producer<Animal>>, Assignable<Producer<Animal>, Producer<Dog>>]
  >().toEqualTypeOf<[true, false]>();
});

// The arrow reverses: something that can swallow any Animal can stand
// in wherever a Dog consumer is wanted.
test("a consumer is contravariant", () => {
  expectTypeOf<
    [Assignable<Consumer<Animal>, Consumer<Dog>>, Assignable<Consumer<Dog>, Consumer<Animal>>]
  >().toEqualTypeOf<[true, false]>();
});

test("a box is invariant: neither direction", () => {
  expectTypeOf<
    [Assignable<Box<Dog>, Box<Animal>>, Assignable<Box<Animal>, Box<Dog>>]
  >().toEqualTypeOf<[false, false]>();
});

test("the members are the ones the shapes need", () => {
  expectTypeOf<ReturnType<Producer<Dog>["get"]>>().toEqualTypeOf<Dog>();
  expectTypeOf<Consumer<Dog>["accept"]>().toEqualTypeOf<(value: Dog) => void>();
  expectTypeOf<Box<Dog>["set"]>().toEqualTypeOf<(value: Dog) => void>();
});

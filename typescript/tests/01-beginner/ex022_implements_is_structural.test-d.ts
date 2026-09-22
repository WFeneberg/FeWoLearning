import { expectTypeOf, test } from "vitest";
import type {
  CollectingLogger,
  SatisfiesLogger,
  SilentLogger,
} from "@ex/01-beginner/ex022_implements_is_structural/index";

// The row in one fact: SilentLogger never declares the interface and is a
// Logger regardless, because the shape is the whole requirement.
test("a class that never declares Logger satisfies it anyway", () => {
  expectTypeOf<SatisfiesLogger<SilentLogger>>().toEqualTypeOf<true>();
});

test("the class that does declare it satisfies it too", () => {
  expectTypeOf<SatisfiesLogger<CollectingLogger>>().toEqualTypeOf<true>();
});

test("a shape missing log does not satisfy it", () => {
  expectTypeOf<SatisfiesLogger<{ prefix: string }>>().toEqualTypeOf<false>();
});

test("a shape whose log takes the wrong argument does not satisfy it", () => {
  expectTypeOf<
    SatisfiesLogger<{ prefix: string; log: (count: number) => void }>
  >().toEqualTypeOf<false>();
});

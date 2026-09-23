import { expectTypeOf, test } from "vitest";
import { Widget, counter } from "@ex/03-advanced/ex080_declaration_merging/index";

test("counter is callable and carries its namespace's exports", () => {
  expectTypeOf(counter).toBeCallableWith();
  expectTypeOf(counter()).toEqualTypeOf<number>();
  expectTypeOf(counter.start).toEqualTypeOf<number>();
  expectTypeOf(counter.reset).toEqualTypeOf<() => void>();
});

test("Widget's type carries the merged member", () => {
  expectTypeOf<Widget["render"]>().toEqualTypeOf<() => string>();
});

test("Widget still has what its own body declares", () => {
  expectTypeOf<keyof Widget>().toEqualTypeOf<"name" | "describe" | "render">();
});

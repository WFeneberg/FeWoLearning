import { expectTypeOf, test } from "vitest";
import { collect, filterAsync, inOrder } from "@ex/02-intermediate/ex060_async_generators/index";

test("inOrder yields the promises' resolved type", () => {
  expectTypeOf(inOrder([Promise.resolve(1)])).toEqualTypeOf<
    AsyncGenerator<number, void, void>
  >();
});

test("collect reports an array of the source's element type", () => {
  expectTypeOf(collect(inOrder([Promise.resolve("a")]))).toEqualTypeOf<Promise<string[]>>();
});

// The predicate's parameter is contextually typed from the source (ex051),
// so this callback needs no annotation.
test("filterAsync keeps the element type and types its predicate", () => {
  expectTypeOf(filterAsync(inOrder([Promise.resolve(1)]), (n) => n > 0)).toEqualTypeOf<
    AsyncGenerator<number, void, void>
  >();
});

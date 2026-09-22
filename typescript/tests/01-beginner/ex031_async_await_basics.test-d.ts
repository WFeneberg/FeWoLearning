import { expectTypeOf, test } from "vitest";
import {
  loadLabel,
  loadOrFallback,
  passThrough,
} from "@ex/01-beginner/ex031_async_await_basics/index";

const loader = async (): Promise<number> => 1;

test("an async function wraps its result in a Promise exactly once", () => {
  expectTypeOf(loadLabel(loader)).toEqualTypeOf<Promise<string>>();
});

// Returning a Promise from an async function does not nest it.
test("returning a Promise from an async function flattens", () => {
  expectTypeOf(passThrough(loader)).toEqualTypeOf<Promise<number>>();
});

test("both branches of the fallback converge on one type", () => {
  expectTypeOf(loadOrFallback(loader)).toEqualTypeOf<Promise<number>>();
});

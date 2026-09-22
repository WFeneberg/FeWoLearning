import { expectTypeOf, test } from "vitest";
import { loadAll } from "@ex/01-beginner/ex033_promise_all_tuple/index";

// The fact that separates an array literal from an array variable: collect
// the three promises into a local first and this becomes
// Promise<(string | number | boolean)[]>, which is not what it says here.
test("loadAll keeps one type per position", () => {
  expectTypeOf(
    loadAll(Promise.resolve("a"), Promise.resolve(1), Promise.resolve(true)),
  ).toEqualTypeOf<Promise<[string, number, boolean]>>();
});

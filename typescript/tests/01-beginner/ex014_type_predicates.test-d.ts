import { expectTypeOf, test } from "vitest";
import { isStringArray, isUser } from "@ex/01-beginner/ex014_type_predicates/index";
import type { User } from "@ex/01-beginner/ex014_type_predicates/index";

// Narrowing at the call site is the only thing that separates a predicate
// from a function that merely returns the right boolean — and a boolean-
// returning stub leaves the value `unknown` inside the branch.
test("isUser narrows its argument", () => {
  const value: unknown = { id: "1", email: "ada@example.com" };
  if (isUser(value)) {
    expectTypeOf(value).toEqualTypeOf<User>();
  }
});

test("isStringArray narrows its argument", () => {
  const value: unknown = ["a", "b"];
  if (isStringArray(value)) {
    expectTypeOf(value).toEqualTypeOf<string[]>();
  }
});

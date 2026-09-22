import { expectTypeOf, test } from "vitest";
import { toError } from "@ex/01-beginner/ex029_catch_is_unknown/index";

// No return annotation on toError, so this grades the body: returning the
// argument unchanged on one path and a new Error on the other has to
// converge on Error, which it only does if the Error branch really builds
// one.
test("toError always produces an Error", () => {
  expectTypeOf(toError("anything")).toEqualTypeOf<Error>();
});

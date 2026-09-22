import { expectTypeOf, test } from "vitest";
import { ValidationError } from "@ex/01-beginner/ex028_error_subclasses/index";

test("field is a string", () => {
  expectTypeOf<ValidationError["field"]>().toEqualTypeOf<string>();
});

test("field cannot be reassigned", () => {
  const error = new ValidationError("bad email", "email");
  // @ts-expect-error — field is readonly
  error.field = "other";
});

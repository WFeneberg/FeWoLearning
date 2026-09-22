import { test } from "vitest";
import { Circle, Shape } from "@ex/01-beginner/ex023_abstract_classes/index";

// Both facts grade a refusal, so both are expect-errors: on the untouched
// stub Shape is a plain class with public methods and these lines compile,
// which leaves the expect-error unused and the fact red.
test("Shape cannot be instantiated", () => {
  // @ts-expect-error — Shape is abstract
  new Shape();
});

test("name is not reachable from outside the hierarchy", () => {
  const circle = new Circle(1);
  // @ts-expect-error — name is protected
  circle.name();
});

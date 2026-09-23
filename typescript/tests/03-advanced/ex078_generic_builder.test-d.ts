import { expectTypeOf, test } from "vitest";
import { configBuilder } from "@ex/03-advanced/ex078_generic_builder/index";
import type { Config } from "@ex/03-advanced/ex078_generic_builder/index";

// The row. `build` is declared with a `this` parameter demanding a
// builder whose accumulated type covers Config, so an incomplete chain
// is not a valid receiver and the error lands on the .build() call.
//
// Both halves in one fact: the stub already declares `build(): Config`,
// so asserting the complete chain's result on its own is green before
// any work is done.
test("build works on a complete chain and not on an incomplete one", () => {
  expectTypeOf(
    configBuilder().set("host", "a").set("port", 1).set("secure", true).build(),
  ).toEqualTypeOf<Config>();
  // @ts-expect-error — secure has not been set
  configBuilder().set("host", "a").set("port", 1).build();
});

test("build is not callable on an empty builder", () => {
  // @ts-expect-error — nothing has been set
  configBuilder().build();
});

// The value is checked against the key that was passed, not against the
// union of every value type (ex049).
test("set checks the value against its own key", () => {
  // @ts-expect-error — port is a number
  configBuilder().set("port", "8080");
});

test("set rejects a key Config does not have", () => {
  // @ts-expect-error — "hsot" is a typo
  configBuilder().set("hsot", "a");
});

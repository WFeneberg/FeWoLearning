import { describe, expect, it } from "vitest";
import { toCss } from "@ex/02-intermediate/ex068_satisfies_operator/index";

// A fact asserting the palette's contents was written and dropped: the
// entries are given in the stub, so it was green on the untouched tree.
// What the stub gets wrong is the TYPE, which the .test-d.ts file grades.

describe("ex068 toCss", () => {
  it("passes a string entry through", () => {
    expect(toCss("green")).toBe("#00ff00");
  });

  it("renders a triple as rgb()", () => {
    expect(toCss("red")).toBe("rgb(255, 0, 0)");
  });
});

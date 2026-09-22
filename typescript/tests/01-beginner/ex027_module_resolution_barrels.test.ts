import { describe, expect, it } from "vitest";
import type { Shape } from "@ex/01-beginner/ex027_module_resolution_barrels/shapes";

// Loaded dynamically and read as a loose record on purpose. A static import
// of a name the stub's barrel does not expose would be a type error in this
// file rather than a failing fact, and the whole subject here is which names
// the barrel exposes.
async function loadBarrel(): Promise<Record<string, unknown>> {
  return (await import(
    "@ex/01-beginner/ex027_module_resolution_barrels/index"
  )) as unknown as Record<string, unknown>;
}

describe("ex027 the barrel", () => {
  // Asserted together on purpose. Measured: this bundler resolves the
  // stub's ambiguous `describe` by keeping the first one, so "describe is
  // the shapes one" is already true on the untouched tree and grades
  // nothing by itself. Pairing it with the renamed member makes the fact
  // red until the barrel is rewritten, and still states what matters —
  // that resolving the clash did not cost the shapes member its name.
  it("keeps describe for shapes and gives the colours one a new name", async () => {
    const barrel = await loadBarrel();
    const describeShape = barrel["describe"] as (shape: Shape) => string;
    const describeColour = barrel["describeColour"] as (c: "red") => string;
    expect(typeof describeColour).toBe("function");
    expect(describeColour("red")).toBe("colour:red");
    expect(describeShape({ kind: "circle", size: 2 })).toBe("circle(2)");

    // Folded in for the same reason: `isWarm` does not clash, so the stub's
    // second `export *` already provides it and a fact of its own would be
    // green before any work was done. What is worth asserting is that
    // rewriting the barrel did not lose it.
    const isWarm = barrel["isWarm"] as (c: "red" | "blue") => boolean;
    expect(isWarm("red")).toBe(true);
    expect(isWarm("blue")).toBe(false);
  });
});

describe("ex027 the import cycle", () => {
  // Dynamic again, for symmetry with the barrel facts above. Measured: the
  // broken cycle does not throw on import — the captured binding is just
  // undefined, and the failure lands here, at the call.
  it("loads a pair of mutually importing modules", async () => {
    const catalogue = await import(
      "@ex/01-beginner/ex027_module_resolution_barrels/catalogue"
    );
    expect(catalogue.describeItem("widget")).toBe("widget: 10");
  });

  it("prices an unknown sku at zero", async () => {
    const pricing = await import(
      "@ex/01-beginner/ex027_module_resolution_barrels/pricing"
    );
    expect(pricing.priceOf("nope")).toBe(0);
  });
});

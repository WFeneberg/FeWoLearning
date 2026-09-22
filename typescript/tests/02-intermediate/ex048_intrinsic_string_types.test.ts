import { describe, expect, it } from "vitest";
import { handlerName } from "@ex/02-intermediate/ex048_intrinsic_string_types/index";

describe("ex048 handlerName", () => {
  it("prefixes and capitalises", () => {
    expect(handlerName("click")).toBe("onClick");
  });

  it("leaves an already-capitalised rest alone", () => {
    expect(handlerName("mouseDown")).toBe("onMouseDown");
  });

  it("handles a single character", () => {
    expect(handlerName("x")).toBe("onX");
  });
});

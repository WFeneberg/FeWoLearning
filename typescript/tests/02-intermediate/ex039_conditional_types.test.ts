import { describe, expect, it } from "vitest";
import { classifyValue } from "@ex/02-intermediate/ex039_conditional_types/index";

describe("ex039 classifyValue", () => {
  it("sorts arrays before objects", () => {
    expect(classifyValue([1, 2])).toBe("array");
    expect(classifyValue([])).toBe("array");
  });

  it("recognises functions", () => {
    expect(classifyValue(() => 1)).toBe("function");
    expect(classifyValue(class {})).toBe("function");
  });

  it("recognises plain objects", () => {
    expect(classifyValue({ a: 1 })).toBe("object");
    expect(classifyValue(new Date())).toBe("object");
  });

  it("treats null as a primitive, not an object", () => {
    expect(classifyValue(null)).toBe("primitive");
  });

  it("recognises primitives", () => {
    expect(classifyValue(1)).toBe("primitive");
    expect(classifyValue("a")).toBe("primitive");
    expect(classifyValue(undefined)).toBe("primitive");
  });
});

import { describe, expect, it } from "vitest";
import { Circle, Rect } from "@ex/01-beginner/ex023_abstract_classes/index";

describe("ex023 Circle", () => {
  it("computes its area", () => {
    expect(new Circle(2).area()).toBeCloseTo(Math.PI * 4, 10);
  });

  it("describes itself through the inherited method", () => {
    expect(new Circle(2).describe()).toBe("circle: 12.57");
  });
});

describe("ex023 Rect", () => {
  it("computes its area", () => {
    expect(new Rect(3, 4).area()).toBe(12);
  });

  it("describes itself through the same inherited method", () => {
    expect(new Rect(3, 4).describe()).toBe("rect: 12.00");
  });
});

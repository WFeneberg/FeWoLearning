import { describe, expect, it } from "vitest";
import { formatCoordinate, head } from "@ex/01-beginner/ex006_tuple_basics/index";

describe("ex006 formatCoordinate", () => {
  it("rounds both components to two decimals", () => {
    expect(formatCoordinate([47.3769, 8.5417])).toBe("47.38,8.54");
  });

  it("pads a whole number to two decimals", () => {
    expect(formatCoordinate([0, -1.5])).toBe("0.00,-1.50");
  });
});

describe("ex006 head", () => {
  it("returns the first of several", () => {
    expect(head(["a", "b", "c"])).toBe("a");
  });

  it("returns the only element of a one-element list", () => {
    expect(head(["solo"])).toBe("solo");
  });
});

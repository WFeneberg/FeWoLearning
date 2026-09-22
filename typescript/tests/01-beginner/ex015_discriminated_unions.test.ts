import { describe, expect, it } from "vitest";
import { render } from "@ex/01-beginner/ex015_discriminated_unions/index";

describe("ex015 render", () => {
  it("renders a click with its coordinates", () => {
    expect(render({ type: "click", x: 10, y: 20 })).toBe("click@10,20");
  });

  it("renders a key press", () => {
    expect(render({ type: "key", key: "Enter" })).toBe("key:Enter");
  });

  it("signs a positive scroll", () => {
    expect(render({ type: "scroll", delta: 3 })).toBe("scroll+3");
  });

  it("signs a negative scroll", () => {
    expect(render({ type: "scroll", delta: -2 })).toBe("scroll-2");
  });

  it("signs a zero scroll as positive", () => {
    expect(render({ type: "scroll", delta: 0 })).toBe("scroll+0");
  });
});

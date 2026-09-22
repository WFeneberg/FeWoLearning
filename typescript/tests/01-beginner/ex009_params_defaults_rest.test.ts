import { describe, expect, it } from "vitest";
import { css, labelAll } from "@ex/01-beginner/ex009_params_defaults_rest/index";

describe("ex009 css", () => {
  it("renders the unit it is given", () => {
    expect(css(4, "em")).toBe("4em");
  });

  it("appends the extras", () => {
    expect(css(4, "em", "!important")).toBe("4em !important");
  });

  // The mechanism fact. A default drops its parameter out of Function.length;
  // a `??` inside the body would leave it at 2 while producing the same
  // strings, so nothing above this line can tell the two apart.
  it("counts only the parameters that have no default", () => {
    expect(css.length).toBe(1);
  });

  it("defaults the unit to px", () => {
    // Called through a widened signature on purpose: omitting `unit` is
    // exactly what the stub's parameter list does not yet permit, and a call
    // the stub rejects would be a type error in this file rather than a
    // failing fact.
    const withDefault = css as unknown as (value: number) => string;
    expect(withDefault(4)).toBe("4px");
  });
});

describe("ex009 labelAll", () => {
  it("prefixes every item", () => {
    expect(labelAll("a:", "x", "y")).toEqual(["a:x", "a:y"]);
  });

  it("returns an empty list when given no items", () => {
    expect(labelAll("a:")).toEqual([]);
  });
});

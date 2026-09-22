import { describe, expect, it } from "vitest";
import {
  Compass,
  Direction,
  enumNames,
  opposite,
} from "@ex/01-beginner/ex011_enum_vs_as_const/index";

describe("ex011 enumNames", () => {
  it("returns the member names of a numeric enum", () => {
    // Object.keys(Direction) has eight entries here, not four: a numeric enum
    // also maps each value back to its name. Filtering those out is the work.
    expect(enumNames(Direction)).toEqual(["Up", "Down", "Left", "Right"]);
  });

  it("leaves a string-valued object alone", () => {
    // A string enum has no reverse mapping, so nothing is filtered.
    expect(enumNames({ A: "a", B: "b" })).toEqual(["A", "B"]);
  });
});

describe("ex011 Compass", () => {
  it("maps each name to its lowercase string", () => {
    expect(Compass).toEqual({ Up: "up", Down: "down", Left: "left", Right: "right" });
  });
});

describe("ex011 opposite", () => {
  it("flips the vertical axis", () => {
    expect(opposite("up")).toBe("down");
    expect(opposite("down")).toBe("up");
  });

  it("flips the horizontal axis", () => {
    expect(opposite("left")).toBe("right");
    expect(opposite("right")).toBe("left");
  });
});

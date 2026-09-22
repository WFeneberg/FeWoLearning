import { describe, expect, it } from "vitest";
import { defaultSettings, withOverrides } from "@ex/01-beginner/ex019_typeof_operator/index";

describe("ex019 withOverrides", () => {
  it("returns the defaults when nothing is overridden", () => {
    const overrides = {};
    expect(withOverrides(overrides)).toEqual({
      retries: 3,
      timeoutMs: 5_000,
      verbose: false,
    });
  });

  it("applies an override on top of the defaults", () => {
    // An inferred local, not a fresh literal: while Settings is still
    // `unknown`, Partial<Settings> is {} and a literal would trip the
    // excess-property check in this file.
    const overrides = { retries: 5 };
    expect(withOverrides(overrides)).toEqual({
      retries: 5,
      timeoutMs: 5_000,
      verbose: false,
    });
  });

  it("applies several overrides", () => {
    const overrides = { retries: 0, verbose: true };
    expect(withOverrides(overrides)).toEqual({
      retries: 0,
      timeoutMs: 5_000,
      verbose: true,
    });
  });

  it("does not mutate the defaults", () => {
    const overrides = { retries: 9 };
    withOverrides(overrides);
    expect(defaultSettings.retries).toBe(3);
  });
});

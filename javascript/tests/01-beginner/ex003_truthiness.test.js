import { describe, expect, it } from "vitest";
import {
  applyDefaults,
  falsyValues,
  orDefault,
  withDefault,
} from "@ex/01-beginner/ex003_truthiness/index.js";

describe("ex003 falsyValues", () => {
  it("lists eight values, all falsy", () => {
    const values = falsyValues();
    expect(values).toHaveLength(8);
    expect(values.every((value) => !value)).toBe(true);
  });

  it("includes the ones people forget", () => {
    const values = falsyValues();
    expect(values.some((value) => Object.is(value, -0))).toBe(true);
    expect(values.some((value) => Object.is(value, 0))).toBe(true);
    expect(values.some((value) => Number.isNaN(value))).toBe(true);
    expect(values).toContain(0n);
    expect(values).toContain("");
    expect(values).toContain(false);
    expect(values).toContain(null);
    expect(values).toContain(undefined);
  });

  it("does not include the near misses", () => {
    const values = falsyValues();
    // Every one of these is truthy, and every one of them surprises someone.
    for (const truthy of ["0", "false", [], {}, Infinity, -1]) {
      expect(values).not.toContainEqual(truthy);
    }
  });
});

describe("ex003 orDefault vs withDefault", () => {
  it("agree on null and undefined", () => {
    expect(orDefault(null, "fb")).toBe("fb");
    expect(withDefault(null, "fb")).toBe("fb");
    expect(orDefault(undefined, "fb")).toBe("fb");
    expect(withDefault(undefined, "fb")).toBe("fb");
  });

  it("disagree on 0, empty string and false — the whole reason ?? exists", () => {
    expect(orDefault(0, 42)).toBe(42);
    expect(withDefault(0, 42)).toBe(0);

    expect(orDefault("", "fb")).toBe("fb");
    expect(withDefault("", "fb")).toBe("");

    expect(orDefault(false, true)).toBe(true);
    expect(withDefault(false, true)).toBe(false);
  });

  it("pass truthy values straight through", () => {
    expect(orDefault("x", "fb")).toBe("x");
    expect(withDefault("x", "fb")).toBe("x");
  });
});

describe("ex003 applyDefaults", () => {
  it("fills the missing keys", () => {
    expect(applyDefaults({})).toEqual({ retries: 3, timeoutMs: 1000, label: "job" });
  });

  it("keeps a supplied 0 or empty string", () => {
    expect(applyDefaults({ retries: 0, label: "" })).toEqual({
      retries: 0,
      timeoutMs: 1000,
      label: "",
    });
  });

  it("replaces an explicit null", () => {
    expect(applyDefaults({ timeoutMs: null }).timeoutMs).toBe(1000);
  });

  it("mutates the object it was given rather than copying it", () => {
    const config = { retries: 7 };
    const result = applyDefaults(config);
    expect(result).toBe(config);
    expect(config.timeoutMs).toBe(1000);
  });

  it("leaves unrelated keys alone", () => {
    expect(applyDefaults({ url: "https://example.test" }).url).toBe("https://example.test");
  });
});

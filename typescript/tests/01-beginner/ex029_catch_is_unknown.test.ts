import { describe, expect, it } from "vitest";
import { toError, tryRun } from "@ex/01-beginner/ex029_catch_is_unknown/index";

describe("ex029 tryRun", () => {
  it("returns the value when nothing goes wrong", () => {
    expect(tryRun(() => "fine")).toBe("fine");
  });

  it("reports a thrown Error by its message", () => {
    expect(
      tryRun(() => {
        throw new Error("boom");
      }),
    ).toBe("error:boom");
  });

  it("reports a thrown string", () => {
    expect(
      tryRun(() => {
        throw "just a string";
      }),
    ).toBe("thrown:just a string");
  });

  it("reports anything else as unknown", () => {
    expect(
      tryRun(() => {
        throw { code: 42 };
      }),
    ).toBe("unknown");
  });

  it("treats a subclass of Error as an Error", () => {
    expect(
      tryRun(() => {
        throw new TypeError("wrong type");
      }),
    ).toBe("error:wrong type");
  });
});

describe("ex029 toError", () => {
  it("passes an Error through unchanged", () => {
    const original = new Error("boom");
    expect(toError(original)).toBe(original);
  });

  // The casts are load-bearing, not laziness. toError carries no return
  // annotation so that the type fact can grade its body, which means the
  // unfinished stub infers `void` — and reading .message off that would be
  // a type error in this file rather than a failing fact.
  it("wraps a string, keeping the original as the cause", () => {
    const wrapped = toError("just a string") as unknown as Error;
    expect(wrapped).toBeInstanceOf(Error);
    expect(wrapped.message).toBe("just a string");
    expect(wrapped.cause).toBe("just a string");
  });

  it("wraps a non-string value", () => {
    const wrapped = toError(42) as unknown as Error;
    expect(wrapped.message).toBe("42");
    expect(wrapped.cause).toBe(42);
  });

  it("wraps null and undefined without throwing", () => {
    expect((toError(null) as unknown as Error).message).toBe("null");
    expect((toError(undefined) as unknown as Error).message).toBe("undefined");
  });
});

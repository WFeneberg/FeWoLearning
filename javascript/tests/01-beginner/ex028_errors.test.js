import { describe, expect, it, vi } from "vitest";
import {
  attempt,
  finallyWins,
  validateAge,
  ValidationError,
} from "@ex/01-beginner/ex028_errors/index.js";

describe("ex028 ValidationError", () => {
  it("is an Error with its own name", () => {
    const error = new ValidationError("bad", "age");
    expect(error).toBeInstanceOf(Error);
    expect(error).toBeInstanceOf(ValidationError);
    expect(error.name).toBe("ValidationError");
    expect(error.message).toBe("bad");
    expect(error.field).toBe("age");
  });

  it("carries a stack, like any Error", () => {
    expect(typeof new ValidationError("bad", "age").stack).toBe("string");
  });

  it("prints its name in toString", () => {
    expect(String(new ValidationError("bad", "age"))).toBe("ValidationError: bad");
  });
});

describe("ex028 validateAge", () => {
  it("passes valid ages through", () => {
    expect(validateAge(0)).toBe(0);
    expect(validateAge(42)).toBe(42);
    expect(validateAge(149)).toBe(149);
  });

  it("rejects non-numbers with a ValidationError", () => {
    // The type matters: a bare toThrow() would be satisfied by the stub's
    // own Error.
    expect(() => validateAge("42")).toThrow(ValidationError);
    expect(() => validateAge(NaN)).toThrow("age must be a number");
    expect(() => validateAge(null)).toThrow(ValidationError);
  });

  it("rejects out-of-range and fractional ages", () => {
    expect(() => validateAge(-1)).toThrow("age out of range");
    expect(() => validateAge(150)).toThrow("age out of range");
    expect(() => validateAge(1.5)).toThrow("age out of range");
  });

  it("tags the offending field", () => {
    try {
      validateAge(-1);
      expect.unreachable("should have thrown");
    } catch (error) {
      expect(error).toBeInstanceOf(ValidationError);
      expect(error.field).toBe("age");
    }
  });
});

describe("ex028 attempt", () => {
  it("reports a return", () => {
    const cleanup = vi.fn();
    expect(attempt(() => 42, cleanup)).toEqual({ ok: true, value: 42 });
    expect(cleanup).toHaveBeenCalledTimes(1);
  });

  it("reports a throw and hands the error back rather than rethrowing", () => {
    const cleanup = vi.fn();
    const boom = new ValidationError("nope", "x");
    const result = attempt(() => {
      throw boom;
    }, cleanup);
    expect(result.ok).toBe(false);
    expect(result.error).toBe(boom);
    expect(cleanup).toHaveBeenCalledTimes(1);
  });

  it("runs the cleanup exactly once, on both paths", () => {
    const cleanup = vi.fn();
    attempt(() => 1, cleanup);
    attempt(() => {
      throw new Error("x");
    }, cleanup);
    expect(cleanup).toHaveBeenCalledTimes(2);
  });

  it("handles a function returning undefined", () => {
    expect(attempt(() => undefined, () => {})).toEqual({ ok: true, value: undefined });
  });
});

describe("ex028 finallyWins", () => {
  it("lets the finally block overwrite the try block's return", () => {
    // Which is exactly why returning from a finally block is a bad habit:
    // it can also swallow an in-flight exception.
    expect(finallyWins()).toBe("finally");
  });
});

import { describe, expect, it } from "vitest";
import {
  causeMessages,
  rootCause,
  runAll,
  wrapErrors,
} from "@ex/02-intermediate/ex044_error_cause/index.js";

const chain = () => {
  const inner = new Error("disk full");
  const middle = new Error("could not write log", { cause: inner });
  return { inner, middle, outer: new Error("request failed", { cause: middle }) };
};

describe("ex044 wrapErrors", () => {
  it("passes a result through untouched", () => {
    expect(wrapErrors(() => 42, "context")).toBe(42);
  });

  it("wraps a failure and keeps the original as the cause", () => {
    const original = new RangeError("out of range");
    try {
      wrapErrors(() => {
        throw original;
      }, "while loading config");
      expect.unreachable("should have thrown");
    } catch (error) {
      expect(error.message).toBe("while loading config");
      expect(error.cause).toBe(original);
    }
  });

  it("keeps the original's own type reachable through the cause", () => {
    try {
      wrapErrors(() => {
        throw new TypeError("bad type");
      }, "outer");
    } catch (error) {
      expect(error).toBeInstanceOf(Error);
      expect(error).not.toBeInstanceOf(TypeError);
      expect(error.cause).toBeInstanceOf(TypeError);
    }
  });
});

describe("ex044 rootCause", () => {
  it("reaches the bottom", () => {
    const { inner, outer } = chain();
    expect(rootCause(outer)).toBe(inner);
  });

  it("returns the error itself when there is no cause", () => {
    const solo = new Error("alone");
    expect(rootCause(solo)).toBe(solo);
  });

  it("works at any depth", () => {
    let error = new Error("level 0");
    for (let i = 1; i <= 10; i++) error = new Error(`level ${i}`, { cause: error });
    expect(rootCause(error).message).toBe("level 0");
  });
});

describe("ex044 causeMessages", () => {
  it("lists the chain outermost first", () => {
    expect(causeMessages(chain().outer)).toEqual([
      "request failed",
      "could not write log",
      "disk full",
    ]);
  });

  it("is a single entry for an unchained error", () => {
    expect(causeMessages(new Error("alone"))).toEqual(["alone"]);
  });
});

describe("ex044 runAll", () => {
  it("returns every result when nothing fails", () => {
    expect(runAll([() => 1, () => 2], "nope")).toEqual([1, 2]);
  });

  it("collects the failures into an AggregateError", () => {
    const first = new Error("first");
    const second = new Error("second");
    try {
      runAll(
        [
          () => {
            throw first;
          },
          () => "fine",
          () => {
            throw second;
          },
        ],
        "two of three failed",
      );
      expect.unreachable("should have thrown");
    } catch (error) {
      expect(error).toBeInstanceOf(AggregateError);
      expect(error.message).toBe("two of three failed");
      expect(error.errors).toEqual([first, second]);
    }
  });

  it("runs every task even after one fails", () => {
    const ran = [];
    try {
      runAll(
        [
          () => {
            ran.push("a");
            throw new Error("x");
          },
          () => ran.push("b"),
        ],
        "partial",
      );
    } catch {
      // expected
    }
    expect(ran).toEqual(["a", "b"]);
  });

  it("returns an empty array for no tasks", () => {
    expect(runAll([], "nope")).toEqual([]);
  });
});

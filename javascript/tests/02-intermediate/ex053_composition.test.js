import { describe, expect, it, vi } from "vitest";
import { compose, pipe, pipeAsync, tap } from "@ex/02-intermediate/ex053_composition/index.js";

const double = (n) => n * 2;
const increment = (n) => n + 1;

describe("ex053 pipe", () => {
  it("applies left to right", () => {
    expect(pipe(double, increment)(5)).toBe(11);
  });

  it("is the identity with no functions", () => {
    expect(pipe()(7)).toBe(7);
    const object = {};
    expect(pipe()(object)).toBe(object);
  });

  it("works with one function", () => {
    expect(pipe(double)(3)).toBe(6);
  });

  it("can change type along the way", () => {
    expect(pipe(String, (s) => s.length)(1234)).toBe(4);
  });
});

describe("ex053 compose", () => {
  it("applies right to left", () => {
    expect(compose(double, increment)(5)).toBe(12);
  });

  it("is pipe's mirror image", () => {
    expect(compose(double, increment)(5)).toBe(pipe(increment, double)(5));
  });

  it("is the identity with no functions", () => {
    expect(compose()(7)).toBe(7);
  });
});

describe("ex053 pipeAsync", () => {
  it("awaits each stage", async () => {
    const fetchLength = async (text) => text.length;
    await expect(pipeAsync(async (n) => String(n), fetchLength)(1234)).resolves.toBe(4);
  });

  it("gives each stage the awaited value, not a promise", async () => {
    const seen = [];
    await pipeAsync(
      async (n) => n + 1,
      (n) => {
        seen.push(n);
        return n;
      },
    )(1);
    expect(seen).toEqual([2]);
  });

  it("mixes sync and async stages", async () => {
    await expect(pipeAsync(double, async (n) => n + 1, double)(5)).resolves.toBe(22);
  });

  it("rejects when a stage rejects, and skips the rest", async () => {
    const later = vi.fn();
    await expect(
      pipeAsync(async () => {
        throw new Error("stage failed");
      }, later)(1),
    ).rejects.toThrow("stage failed");
    expect(later).not.toHaveBeenCalled();
  });
});

describe("ex053 tap", () => {
  it("returns the value untouched", () => {
    const object = { a: 1 };
    expect(tap(() => "ignored")(object)).toBe(object);
  });

  it("runs the side effect with the value", () => {
    const spy = vi.fn();
    expect(pipe(double, tap(spy), increment)(5)).toBe(11);
    expect(spy).toHaveBeenCalledWith(10);
  });
});

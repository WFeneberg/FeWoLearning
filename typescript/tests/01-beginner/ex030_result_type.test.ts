import { describe, expect, it } from "vitest";
import { err, mapResult, ok, unwrapOr } from "@ex/01-beginner/ex030_result_type/index";

describe("ex030 ok and err", () => {
  it("builds a success", () => {
    expect(ok(42)).toEqual({ ok: true, value: 42 });
  });

  it("builds a failure", () => {
    expect(err("nope")).toEqual({ ok: false, error: "nope" });
  });

  it("uses a literal discriminant, not a truthy value", () => {
    expect((ok(1) as { ok: unknown }).ok).toBe(true);
    expect((err("x") as { ok: unknown }).ok).toBe(false);
  });
});

describe("ex030 mapResult", () => {
  it("applies the function to a success", () => {
    expect(mapResult(ok(2) as never, ((n: number) => n * 3) as never)).toEqual({
      ok: true,
      value: 6,
    });
  });

  it("passes a failure through without calling the function", () => {
    let called = false;
    const mapped = mapResult(err("nope") as never, ((n: number) => {
      called = true;
      return n;
    }) as never);
    expect(mapped).toEqual({ ok: false, error: "nope" });
    expect(called).toBe(false);
  });

  it("can change the value type", () => {
    expect(mapResult(ok(2) as never, ((n: number) => `n=${n}`) as never)).toEqual({
      ok: true,
      value: "n=2",
    });
  });
});

describe("ex030 unwrapOr", () => {
  it("returns a success's value", () => {
    expect(unwrapOr(ok(42) as never, 0 as never)).toBe(42);
  });

  it("returns the fallback for a failure", () => {
    expect(unwrapOr(err("nope") as never, 0 as never)).toBe(0);
  });
});

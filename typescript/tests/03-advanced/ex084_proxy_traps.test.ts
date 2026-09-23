import { describe, expect, it } from "vitest";
import { hidingUnderscored, recording, validated } from "@ex/03-advanced/ex084_proxy_traps/index";
import type { Access } from "@ex/03-advanced/ex084_proxy_traps/index";

describe("ex084 recording", () => {
  it("records each read, in order, including repeats", () => {
    const access: Access = { reads: [] };
    const proxy = recording({ a: 1, b: 2 }, access);
    void proxy.a;
    void proxy.b;
    void proxy.a;
    expect(access.reads).toEqual(["a", "b", "a"]);
  });

  it("still returns the real values", () => {
    const access: Access = { reads: [] };
    const proxy = recording({ a: 1, b: "x" }, access);
    expect(proxy.a).toBe(1);
    expect(proxy.b).toBe("x");
  });

  // Reflect with the receiver is what makes a prototype accessor work.
  it("works for a getter inherited from a prototype", () => {
    const access: Access = { reads: [] };
    const base = {
      get doubled(): number {
        return (this as unknown as { n: number }).n * 2;
      },
    };
    const target = Object.create(base) as unknown as { n: number; doubled: number };
    target.n = 21;
    const proxy = recording(target, access);
    expect(proxy.doubled).toBe(42);
  });
});

describe("ex084 validated", () => {
  it("allows a write the validator accepts", () => {
    const target = { count: 0 };
    const proxy = validated(target, (_key, value) => typeof value === "number");
    proxy.count = 5;
    expect(target.count).toBe(5);
  });

  it("throws on a write the validator rejects", () => {
    const target = { count: 0 };
    const proxy = validated(target, (_key, value) => typeof value === "number");
    expect(() => {
      (proxy as unknown as Record<string, unknown>)["count"] = "nope";
    }).toThrow(TypeError);
  });

  it("leaves the target untouched after a rejection", () => {
    const target = { count: 7 };
    const proxy = validated(target, () => false);
    expect(() => {
      proxy.count = 9;
    }).toThrow(TypeError);
    expect(target.count).toBe(7);
  });

  it("can validate by key as well as by value", () => {
    const target: Record<string, unknown> = {};
    const proxy = validated(target, (key) => !key.startsWith("_"));
    proxy["ok"] = 1;
    expect(() => {
      proxy["_secret"] = 1;
    }).toThrow(TypeError);
    expect(target).toEqual({ ok: 1 });
  });
});

describe("ex084 hidingUnderscored", () => {
  it("hides an underscored key from `in`", () => {
    const proxy = hidingUnderscored({ a: 1, _b: 2 });
    expect("a" in proxy).toBe(true);
    expect("_b" in proxy).toBe(false);
  });

  // Hidden from `in`, still readable — which is the point of doing this
  // with a trap rather than by deleting the key.
  it("still reads the hidden key", () => {
    const proxy = hidingUnderscored({ _b: 2 });
    expect(proxy._b).toBe(2);
  });

  it("reports false for a key that is genuinely absent", () => {
    expect("zz" in hidingUnderscored({ a: 1 })).toBe(false);
  });
});

import { describe, expect, it } from "vitest";
import { Money, Sequence, tagOf } from "@ex/03-advanced/ex083_well_known_symbols/index";

describe("ex083 Symbol.toPrimitive", () => {
  it("uses the number hint for arithmetic", () => {
    expect(Number(new Money(42, "CHF"))).toBe(42);
    expect(new Money(10, "CHF") as unknown as number * 2).toBe(20);
  });

  it("uses the string hint for interpolation", () => {
    expect(`${new Money(42, "CHF")}`).toBe("42 CHF");
    expect(String(new Money(42, "CHF"))).toBe("42 CHF");
  });

  // "default" is the hint `+` and `==` pass, and handling only the other
  // two is the usual bug.
  it("renders rather than adding under the default hint", () => {
    expect((new Money(42, "CHF") as unknown as string) + "").toBe("42 CHF");
  });
});

describe("ex083 Symbol.toStringTag", () => {
  // Paired with the untouched cases: "leaves other objects alone" is
  // true before any work is done, and grades nothing on its own.
  it("renames only itself", () => {
    expect([tagOf(new Money(1, "CHF")), tagOf({}), tagOf([])]).toEqual([
      "Money",
      "Object",
      "Array",
    ]);
  });
});

describe("ex083 Symbol.hasInstance", () => {
  // A primitive on the left of instanceof is a type error, so the
  // operands go through a helper. The operator still does the work.
  const isSequence = (value: unknown): boolean =>
    (value as object) instanceof Sequence;

  // instanceof takes the static method's word for it, prototype chain or
  // no prototype chain. Accepted and rejected together, since an
  // untouched Sequence already rejects everything.
  it("answers for anything iterable and for nothing else", () => {
    expect([
      isSequence([]),
      isSequence("abc"),
      isSequence(new Set([1])),
      isSequence(new Map()),
      isSequence({}),
      isSequence(42),
      isSequence(null),
      isSequence(undefined),
    ]).toEqual([true, true, true, true, false, false, false, false]);
  });
});

import { describe, expect, it } from "vitest";
import {
  hiddenKey,
  money,
  symbolIdentity,
  taggedObject,
} from "@ex/02-intermediate/ex051_symbols/index.js";

describe("ex051 symbolIdentity", () => {
  it("makes every Symbol() unique, and every Symbol.for() shared", () => {
    expect(symbolIdentity()).toEqual({
      same: false,
      equal: true,
      registeredSame: true,
      keyFor: "x",
    });
  });
});

describe("ex051 hiddenKey", () => {
  it("stores and reads the value under the symbol", () => {
    const { object, key } = hiddenKey("secret");
    expect(typeof key).toBe("symbol");
    expect(object[key]).toBe("secret");
  });

  it("is invisible to every string-key route", () => {
    const { object } = hiddenKey("secret");
    expect(Object.keys(object)).toEqual(["visible"]);
    expect(JSON.stringify(object)).toBe('{"visible":1}');
    expect(Object.getOwnPropertyNames(object)).toEqual(["visible"]);
    expect(Object.entries(object)).toEqual([["visible", 1]]);
  });

  it("is findable through the symbol-aware reflection", () => {
    const { object, key } = hiddenKey("secret");
    expect(Object.getOwnPropertySymbols(object)).toEqual([key]);
    expect(Reflect.ownKeys(object)).toEqual(["visible", key]);
  });

  it("survives a spread, which copies symbol keys too", () => {
    const { object, key } = hiddenKey("secret");
    expect({ ...object }[key]).toBe("secret");
  });
});

describe("ex051 taggedObject", () => {
  it("changes what Object.prototype.toString reports", () => {
    expect(Object.prototype.toString.call(taggedObject("Money"))).toBe("[object Money]");
    expect(Object.prototype.toString.call({})).toBe("[object Object]");
  });

  it("does not add a visible property", () => {
    expect(Object.keys(taggedObject("Money"))).toEqual([]);
  });
});

describe("ex051 money", () => {
  it("coerces to a number where a number is wanted", () => {
    expect(+money(1250)).toBe(12.5);
    expect(money(1250) * 2).toBe(25);
    expect(Number(money(50))).toBe(0.5);
  });

  it("coerces to a string where a string is wanted", () => {
    expect(`${money(1250)}`).toBe("12.50 CHF");
    expect(String(money(5))).toBe("0.05 CHF");
  });

  it("uses the string form for the default hint", () => {
    // `+` with no numeric partner and template concatenation both ask for
    // "default".
    expect(money(1250) + "").toBe("12.50 CHF");
  });

  it("still exposes the raw cents", () => {
    expect(money(1250).cents).toBe(1250);
  });
});

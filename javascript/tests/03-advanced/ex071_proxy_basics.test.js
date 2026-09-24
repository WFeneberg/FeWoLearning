import { describe, expect, it } from "vitest";
import { counting, hide, strict, validated } from "@ex/03-advanced/ex071_proxy_basics/index.js";

describe("ex071 strict", () => {
  it("reads what is there", () => {
    expect(strict({ a: 1 }).a).toBe(1);
  });

  it("throws for a key that is not", () => {
    const view = strict({ colour: "red" });
    expect(() => view.color).toThrow(ReferenceError);
    expect(() => view.color).toThrow("unknown property: color");
  });

  it("accepts inherited keys", () => {
    expect(typeof strict({}).toString).toBe("function");
  });

  it("lets symbol probes through, so the object stays usable", () => {
    // Without the symbol exemption, a spread of the proxy throws on
    // Symbol.iterator before it copies a single key.
    const view = strict({ a: 1 });
    expect(() => ({ ...view })).not.toThrow();
    expect({ ...view }).toEqual({ a: 1 });
  });

  it("is still hostile to JSON.stringify, which probes a STRING key", () => {
    // Measured: JSON.stringify reads "toJSON" before serialising, and that
    // is not a symbol, so a strict view throws. Worth knowing before
    // wrapping anything that will be logged.
    expect(() => JSON.stringify(strict({ a: 1 }))).toThrow(ReferenceError);
  });

  it("does not shadow the target", () => {
    const target = { a: 1 };
    strict(target).a = 2;
    expect(target.a).toBe(2);
  });
});

describe("ex071 validated", () => {
  const rules = { age: (value) => Number.isInteger(value) && value >= 0 };

  it("accepts a valid write", () => {
    const target = {};
    const view = validated(target, rules);
    view.age = 30;
    expect(view.age).toBe(30);
    expect(target.age).toBe(30);
  });

  it("rejects an invalid one", () => {
    const view = validated({}, rules);
    expect(() => {
      view.age = -1;
    }).toThrow(TypeError);
    expect(() => {
      view.age = "thirty";
    }).toThrow("invalid value for age");
  });

  it("leaves the target unchanged after a rejected write", () => {
    const target = { age: 30 };
    const view = validated(target, rules);
    try {
      view.age = -1;
    } catch {
      // expected
    }
    expect(target.age).toBe(30);
  });

  it("passes unvalidated keys through", () => {
    const view = validated({}, rules);
    view.anything = -999;
    expect(view.anything).toBe(-999);
  });
});

describe("ex071 counting", () => {
  it("counts reads per key", () => {
    const { view, counts } = counting({ a: 1, b: 2 });
    void view.a;
    void view.a;
    void view.b;
    expect(counts.get("a")).toBe(2);
    expect(counts.get("b")).toBe(1);
  });

  it("does not count writes", () => {
    const { view, counts } = counting({ a: 1 });
    view.a = 5;
    expect(counts.get("a")).toBeUndefined();
  });

  it("counts a read of a missing key too", () => {
    const { view, counts } = counting({});
    void view.missing;
    expect(counts.get("missing")).toBe(1);
  });

  it("still returns the values", () => {
    const { view } = counting({ a: 1 });
    expect(view.a).toBe(1);
  });
});

describe("ex071 hide", () => {
  it("hides from reads", () => {
    const view = hide({ visible: 1, secret: 2 }, ["secret"]);
    expect(view.visible).toBe(1);
    expect(view.secret).toBeUndefined();
  });

  it("hides from `in` and from enumeration", () => {
    const view = hide({ visible: 1, secret: 2 }, ["secret"]);
    expect("secret" in view).toBe(false);
    expect("visible" in view).toBe(true);
    expect(Object.keys(view)).toEqual(["visible"]);
    expect({ ...view }).toEqual({ visible: 1 });
    expect(JSON.stringify(view)).toBe('{"visible":1}');
  });

  it("swallows a delete of a hidden key", () => {
    const target = { secret: 2 };
    const view = hide(target, ["secret"]);
    expect(delete view.secret).toBe(true);
    expect(target.secret).toBe(2);
  });

  it("still deletes a visible key", () => {
    const target = { visible: 1 };
    delete hide(target, ["secret"]).visible;
    expect(Object.hasOwn(target, "visible")).toBe(false);
  });
});

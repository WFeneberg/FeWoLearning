import { describe, expect, it } from "vitest";
import {
  bareObject,
  chainOf,
  createWith,
  describeProperty,
  shadow,
} from "@ex/02-intermediate/ex045_prototype_chain/index.js";

describe("ex045 createWith", () => {
  it("links the prototype and adds the own properties", () => {
    const proto = { greet: () => "hi" };
    const object = createWith(proto, { name: "Ada" });
    expect(Object.getPrototypeOf(object)).toBe(proto);
    expect(object.name).toBe("Ada");
    expect(object.greet()).toBe("hi");
    expect(Object.keys(object)).toEqual(["name"]);
  });

  it("does not copy the prototype's properties onto the object", () => {
    const object = createWith({ shared: 1 }, {});
    expect(object.shared).toBe(1);
    expect(Object.hasOwn(object, "shared")).toBe(false);
  });

  it("accepts null as the prototype", () => {
    expect(Object.getPrototypeOf(createWith(null, { a: 1 }))).toBeNull();
  });
});

describe("ex045 chainOf", () => {
  it("ends at null", () => {
    expect(chainOf({})).toEqual([Object.prototype, null]);
  });

  it("reports every link for a class instance", () => {
    class Base {}
    class Derived extends Base {}
    expect(chainOf(new Derived())).toEqual([
      Derived.prototype,
      Base.prototype,
      Object.prototype,
      null,
    ]);
  });

  it("is just [null] for a bare object", () => {
    expect(chainOf(Object.create(null))).toEqual([null]);
  });

  it("works for an array and for a primitive", () => {
    expect(chainOf([])[0]).toBe(Array.prototype);
    expect(chainOf("str")[0]).toBe(String.prototype);
  });
});

describe("ex045 describeProperty", () => {
  it("tells own from inherited", () => {
    const child = createWith({ inherited: "from proto" }, { own: "mine" });
    expect(describeProperty(child, "own")).toEqual({
      own: true,
      inChain: true,
      value: "mine",
    });
    expect(describeProperty(child, "inherited")).toEqual({
      own: false,
      inChain: true,
      value: "from proto",
    });
  });

  it("finds nothing for a key nobody has", () => {
    expect(describeProperty({}, "nope")).toEqual({
      own: false,
      inChain: false,
      value: undefined,
    });
  });

  it("finds toString in the chain of any plain object", () => {
    const result = describeProperty({}, "toString");
    expect(result.own).toBe(false);
    expect(result.inChain).toBe(true);
  });
});

describe("ex045 shadow", () => {
  it("writes on the object and leaves the prototype alone", () => {
    const proto = { level: "proto" };
    const child = Object.create(proto);
    expect(shadow(child, "level", "own")).toEqual({
      objectValue: "own",
      protoValue: "proto",
    });
    expect(proto.level).toBe("proto");
    expect(Object.hasOwn(child, "level")).toBe(true);
  });

  it("does not affect siblings that share the prototype", () => {
    const proto = { level: "proto" };
    const first = Object.create(proto);
    const second = Object.create(proto);
    shadow(first, "level", "changed");
    expect(second.level).toBe("proto");
  });

  it("can add a property the prototype never had", () => {
    expect(shadow(Object.create({}), "fresh", 1)).toEqual({
      objectValue: 1,
      protoValue: undefined,
    });
  });
});

describe("ex045 bareObject", () => {
  it("has the payload and no prototype", () => {
    const bare = bareObject();
    expect(bare.safe).toBe(true);
    expect(Object.getPrototypeOf(bare)).toBeNull();
  });

  it("inherits nothing at all", () => {
    const bare = bareObject();
    expect("toString" in bare).toBe(false);
    expect(bare.hasOwnProperty).toBeUndefined();
    expect(() => String(bare)).toThrow(TypeError);
  });

  it("is safe to use as a lookup table", () => {
    // On a plain object, every one of these keys already answers something.
    const bare = bareObject();
    for (const key of ["constructor", "toString", "__proto__"]) {
      expect(bare[key]).toBeUndefined();
    }
  });
});

import { describe, expect, it } from "vitest";
import {
  defineConstant,
  definedVsAssigned,
  defineHidden,
  flagsOf,
  writeToReadOnly,
} from "@ex/02-intermediate/ex047_property_descriptors/index.js";

describe("ex047 defineConstant", () => {
  it("is readable and visible", () => {
    const object = defineConstant({}, "VERSION", "1.0");
    expect(object.VERSION).toBe("1.0");
    expect(Object.keys(object)).toEqual(["VERSION"]);
    expect(JSON.stringify(object)).toBe('{"VERSION":"1.0"}');
  });

  it("returns the object it was given", () => {
    const object = {};
    expect(defineConstant(object, "A", 1)).toBe(object);
  });

  it("rejects a write", () => {
    const object = defineConstant({}, "A", 1);
    expect(writeToReadOnly(object, "A", 2)).toBe("TypeError");
    expect(object.A).toBe(1);
  });

  it("cannot be redefined or deleted", () => {
    const object = defineConstant({}, "A", 1);
    expect(() => Object.defineProperty(object, "A", { value: 2 })).toThrow(TypeError);
    expect(() => {
      delete object.A;
    }).toThrow(TypeError);
  });
});

describe("ex047 defineHidden", () => {
  it("is readable but invisible to enumeration", () => {
    const object = defineHidden({ shown: 1 }, "internal", "x");
    expect(object.internal).toBe("x");
    expect(Object.keys(object)).toEqual(["shown"]);
    expect(JSON.stringify(object)).toBe('{"shown":1}');
    const seen = [];
    for (const key in object) seen.push(key);
    expect(seen).toEqual(["shown"]);
  });

  it("is still writable and deletable", () => {
    const object = defineHidden({}, "internal", "x");
    object.internal = "y";
    expect(object.internal).toBe("y");
    delete object.internal;
    expect(Object.hasOwn(object, "internal")).toBe(false);
  });

  it("is findable by the reflection that ignores enumerability", () => {
    const object = defineHidden({}, "internal", "x");
    expect(Object.getOwnPropertyNames(object)).toEqual(["internal"]);
    expect("internal" in object).toBe(true);
  });
});

describe("ex047 flagsOf", () => {
  it("reports the three flags", () => {
    expect(flagsOf({ a: 1 }, "a")).toEqual({
      writable: true,
      enumerable: true,
      configurable: true,
    });
  });

  it("is null for a key the object does not own", () => {
    expect(flagsOf({}, "missing")).toBeNull();
    expect(flagsOf({}, "toString")).toBeNull();
  });
});

describe("ex047 definedVsAssigned", () => {
  it("shows defineProperty defaulting everything to false", () => {
    const { defined, assigned } = definedVsAssigned();
    expect(defined).toEqual({ writable: false, enumerable: false, configurable: false });
    expect(assigned).toEqual({ writable: true, enumerable: true, configurable: true });
  });
});

describe("ex047 writeToReadOnly", () => {
  it("throws for a non-writable property in strict mode", () => {
    const object = Object.defineProperty({}, "a", { value: 1 });
    expect(writeToReadOnly(object, "a", 2)).toBe("TypeError");
  });

  it("says so when the write was allowed after all", () => {
    expect(writeToReadOnly({ a: 1 }, "a", 2)).toBe("no error");
  });
});

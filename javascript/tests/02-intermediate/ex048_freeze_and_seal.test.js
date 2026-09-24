import { describe, expect, it } from "vitest";
import {
  classify,
  freezeDeep,
  probeMutations,
  shallowGap,
} from "@ex/02-intermediate/ex048_freeze_and_seal/index.js";

describe("ex048 classify", () => {
  it("describes a plain object", () => {
    expect(classify({ a: 1 })).toEqual({ frozen: false, sealed: false, extensible: true });
  });

  it("describes the three levels", () => {
    expect(classify(Object.preventExtensions({ a: 1 }))).toEqual({
      frozen: false,
      sealed: false,
      extensible: false,
    });
    expect(classify(Object.seal({ a: 1 }))).toEqual({
      frozen: false,
      sealed: true,
      extensible: false,
    });
    expect(classify(Object.freeze({ a: 1 }))).toEqual({
      frozen: true,
      sealed: true,
      extensible: false,
    });
  });

  it("calls an empty non-extensible object frozen, since it has nothing to protect", () => {
    expect(classify(Object.preventExtensions({}))).toEqual({
      frozen: true,
      sealed: true,
      extensible: false,
    });
  });
});

describe("ex048 freezeDeep", () => {
  it("freezes every level", () => {
    const object = freezeDeep({ a: { b: { c: 1 } }, list: [{ d: 2 }] });
    expect(Object.isFrozen(object)).toBe(true);
    expect(Object.isFrozen(object.a)).toBe(true);
    expect(Object.isFrozen(object.a.b)).toBe(true);
    expect(Object.isFrozen(object.list)).toBe(true);
    expect(Object.isFrozen(object.list[0])).toBe(true);
  });

  it("returns the object it was given", () => {
    const object = { a: 1 };
    expect(freezeDeep(object)).toBe(object);
  });

  it("survives a cycle", () => {
    const object = { name: "self" };
    object.self = object;
    expect(Object.isFrozen(freezeDeep(object))).toBe(true);
  });

  it("survives shared references without visiting them twice forever", () => {
    const shared = { x: 1 };
    const object = freezeDeep({ a: shared, b: shared });
    expect(Object.isFrozen(object.a)).toBe(true);
    expect(object.a).toBe(object.b);
  });

  it("leaves primitives alone", () => {
    expect(freezeDeep(5)).toBe(5);
    expect(freezeDeep(null)).toBeNull();
  });
});

describe("ex048 shallowGap", () => {
  it("shows freeze reaching exactly one level", () => {
    expect(shallowGap()).toEqual({ frozenOuter: true, nestedValue: 2 });
  });
});

describe("ex048 probeMutations", () => {
  it("allows everything on a plain object", () => {
    expect(probeMutations({ a: 1 })).toEqual({ write: "ok", addKey: "ok", deleteKey: "ok" });
  });

  it("blocks only additions on a non-extensible object", () => {
    expect(probeMutations(Object.preventExtensions({ a: 1 }))).toEqual({
      write: "ok",
      addKey: "TypeError",
      deleteKey: "ok",
    });
  });

  it("blocks additions and deletions on a sealed object", () => {
    expect(probeMutations(Object.seal({ a: 1 }))).toEqual({
      write: "ok",
      addKey: "TypeError",
      deleteKey: "TypeError",
    });
  });

  it("blocks all three on a frozen object", () => {
    expect(probeMutations(Object.freeze({ a: 1 }))).toEqual({
      write: "TypeError",
      addKey: "TypeError",
      deleteKey: "TypeError",
    });
  });
});

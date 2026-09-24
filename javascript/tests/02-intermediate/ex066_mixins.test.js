import { describe, expect, it } from "vitest";
import {
  copyMembers,
  mix,
  withSerializable,
  withStamp,
} from "@ex/02-intermediate/ex066_mixins/index.js";

class Note {
  constructor(text) {
    this.text = text;
  }

  preview() {
    return this.text.slice(0, 3);
  }
}

describe("ex066 withSerializable", () => {
  it("adds behaviour and keeps the base's", () => {
    const SerializableNote = withSerializable(Note);
    const note = new SerializableNote("hello");
    expect(note.preview()).toBe("hel");
    expect(note.toJSON()).toEqual({ text: "hello" });
    expect(note.serialized).toBe('{"text":"hello"}');
  });

  it("leaves the base class untouched", () => {
    withSerializable(Note);
    expect(new Note("x").toJSON).toBeUndefined();
  });

  it("produces a real subclass", () => {
    const SerializableNote = withSerializable(Note);
    expect(new SerializableNote("x")).toBeInstanceOf(Note);
  });

  it("is applied through JSON.stringify as well", () => {
    const SerializableNote = withSerializable(Note);
    expect(JSON.stringify(new SerializableNote("x"))).toBe('{"text":"x"}');
  });
});

describe("ex066 withStamp", () => {
  it("adds a chainable stamp", () => {
    const StampedNote = withStamp(Note);
    const note = new StampedNote("x");
    expect(note.stamp(123)).toBe(note);
    expect(note.updatedAt).toBe(123);
  });
});

describe("ex066 mix", () => {
  it("applies mixins left to right", () => {
    const Mixed = mix(Note, withSerializable, withStamp);
    const note = new Mixed("hello");
    expect(note.preview()).toBe("hel");
    expect(note.stamp(7).toJSON()).toEqual({ text: "hello", updatedAt: 7 });
  });

  it("returns the base class untouched with no mixins", () => {
    expect(mix(Note)).toBe(Note);
  });

  it("stacks the prototypes rather than merging them", () => {
    const Mixed = mix(Note, withSerializable, withStamp);
    expect(Object.hasOwn(Mixed.prototype, "stamp")).toBe(true);
    expect(Object.hasOwn(Mixed.prototype, "toJSON")).toBe(false);
    expect(new Mixed("x").toJSON).toBeTypeOf("function");
  });
});

describe("ex066 copyMembers", () => {
  it("copies plain values", () => {
    const target = copyMembers({}, { a: 1, b: 2 });
    expect(target).toEqual({ a: 1, b: 2 });
  });

  it("returns the target", () => {
    const target = {};
    expect(copyMembers(target, { a: 1 })).toBe(target);
  });

  it("keeps a getter live, where Object.assign would freeze its value", () => {
    let count = 0;
    const source = {
      get next() {
        return ++count;
      },
    };
    const copied = copyMembers({}, source);
    expect(copied.next).toBe(1);
    expect(copied.next).toBe(2);

    count = 0;
    const assigned = Object.assign({}, source);
    expect(assigned.next).toBe(1);
    expect(assigned.next).toBe(1);
  });

  it("copies non-enumerable members too", () => {
    const source = Object.defineProperty({}, "hidden", { value: 1, enumerable: false });
    const copied = copyMembers({}, source);
    expect(copied.hidden).toBe(1);
    expect(Object.keys(copied)).toEqual([]);
    expect(Object.keys(Object.assign({}, source))).toEqual([]);
    expect(Object.assign({}, source).hidden).toBeUndefined();
  });

  it("copies symbol-keyed members", () => {
    const key = Symbol("k");
    expect(copyMembers({}, { [key]: "v" })[key]).toBe("v");
  });
});

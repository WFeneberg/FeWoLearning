import { describe, expect, it } from "vitest";
import { matches, parse } from "@ex/04-expert/ex100_schema_inference/index";
import type { Schema } from "@ex/04-expert/ex100_schema_inference/index";

const userSchema = {
  kind: "object",
  fields: {
    id: { kind: "string" },
    age: { kind: "number" },
    nickname: { kind: "optional", of: { kind: "string" } },
    tags: { kind: "array", of: { kind: "string" } },
  },
} as const satisfies Schema;

describe("ex100 matches", () => {
  it("accepts the primitives", () => {
    expect(matches({ kind: "string" }, "a")).toBe(true);
    expect(matches({ kind: "number" }, 1)).toBe(true);
    expect(matches({ kind: "boolean" }, false)).toBe(true);
  });

  it("rejects the wrong primitive", () => {
    expect(matches({ kind: "string" }, 1)).toBe(false);
    expect(matches({ kind: "number" }, "1")).toBe(false);
  });

  it("checks an array's elements, not just the container", () => {
    const schema = { kind: "array", of: { kind: "number" } } as const satisfies Schema;
    expect(matches(schema, [1, 2])).toBe(true);
    expect(matches(schema, [])).toBe(true);
    expect(matches(schema, [1, "2"])).toBe(false);
    expect(matches(schema, "not an array")).toBe(false);
  });

  it("lets an optional field be absent or present", () => {
    const schema = { kind: "optional", of: { kind: "string" } } as const satisfies Schema;
    expect(matches(schema, undefined)).toBe(true);
    expect(matches(schema, "a")).toBe(true);
    expect(matches(schema, 1)).toBe(false);
  });

  it("accepts a complete object", () => {
    expect(
      matches(userSchema, { id: "u", age: 1, nickname: "n", tags: ["a"] }),
    ).toBe(true);
  });

  it("accepts an object with the optional field left out", () => {
    expect(matches(userSchema, { id: "u", age: 1, tags: [] })).toBe(true);
  });

  it("rejects a missing required field", () => {
    expect(matches(userSchema, { id: "u", tags: [] })).toBe(false);
  });

  it("rejects a field of the wrong type", () => {
    expect(matches(userSchema, { id: "u", age: "1", tags: [] })).toBe(false);
  });

  it("rejects a non-object where an object is expected", () => {
    expect(matches(userSchema, null)).toBe(false);
    expect(matches(userSchema, [])).toBe(false);
    expect(matches(userSchema, "u")).toBe(false);
  });
});

describe("ex100 parse", () => {
  it("gives the value back when it matches", () => {
    const value = { id: "u", age: 1, tags: ["a"] };
    expect(parse(userSchema, value)).toBe(value);
  });

  it("gives undefined when it does not", () => {
    expect(parse(userSchema, { id: "u" })).toBeUndefined();
  });
});

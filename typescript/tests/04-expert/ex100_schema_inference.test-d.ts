import { expectTypeOf, test } from "vitest";
import { parse } from "@ex/04-expert/ex100_schema_inference/index";
import type { Infer, Schema } from "@ex/04-expert/ex100_schema_inference/index";

const userSchema = {
  kind: "object",
  fields: {
    id: { kind: "string" },
    age: { kind: "number" },
    nickname: { kind: "optional", of: { kind: "string" } },
    tags: { kind: "array", of: { kind: "string" } },
  },
} as const satisfies Schema;

type User = Infer<typeof userSchema>;

/** Which keys may be left out entirely — the ex005 probe. */
type OptionalKeys<T> = {
  [K in keyof T]-?: {} extends Pick<T, K> ? K : never;
}[keyof T];

test("the primitives infer themselves", () => {
  expectTypeOf<Infer<{ kind: "string" }>>().toEqualTypeOf<string>();
  expectTypeOf<Infer<{ kind: "number" }>>().toEqualTypeOf<number>();
  expectTypeOf<Infer<{ kind: "boolean" }>>().toEqualTypeOf<boolean>();
});

test("an array infers an array of its element type", () => {
  expectTypeOf<Infer<{ kind: "array"; of: { kind: "number" } }>>().toEqualTypeOf<number[]>();
});

test("an optional infers the union with undefined", () => {
  expectTypeOf<Infer<{ kind: "optional"; of: { kind: "string" } }>>().toEqualTypeOf<
    string | undefined
  >();
});

test("an object infers one property per field", () => {
  expectTypeOf<User["id"]>().toEqualTypeOf<string>();
  expectTypeOf<User["age"]>().toEqualTypeOf<number>();
  expectTypeOf<User["tags"]>().toEqualTypeOf<string[]>();
});

// The interesting half: a mapped type carries ONE modifier for all its
// keys, so this only works if required and optional are built apart
// and intersected.
test("only the optional field may be omitted", () => {
  expectTypeOf<OptionalKeys<User>>().toEqualTypeOf<"nickname">();
});

test("nesting goes all the way down", () => {
  const nested = {
    kind: "object",
    fields: { inner: { kind: "object", fields: { n: { kind: "number" } } } },
  } as const satisfies Schema;
  expectTypeOf<Infer<typeof nested>["inner"]["n"]>().toEqualTypeOf<number>();
});

// Read through a member rather than as `User | undefined`: while Infer
// is still `unknown`, so is User, and `unknown | undefined` collapses
// back to unknown — the fact would compare unknown with unknown and
// pass before any work was done (the ex030 trap).
test("parse reports the schema's own type", () => {
  const result = parse(userSchema, {});
  if (result !== undefined) {
    expectTypeOf(result.id).toEqualTypeOf<string>();
    expectTypeOf(result.tags).toEqualTypeOf<string[]>();
  }
});

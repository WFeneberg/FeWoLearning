import { expectTypeOf, test } from "vitest";
import { Entity, Taggable, Timestamped } from "@ex/03-advanced/ex082_class_mixins/index";

// The mixin's return type is inferred, so the combined instance type is
// worked out at each call rather than written anywhere.
test("Timestamped adds its members and keeps the base's", () => {
  const instance = new (Timestamped(Entity))("e-1");
  expectTypeOf(instance.createdAt).toEqualTypeOf<number>();
  expectTypeOf(instance.age(0)).toEqualTypeOf<number>();
  expectTypeOf(instance.id).toEqualTypeOf<string>();
  expectTypeOf(instance.describe()).toEqualTypeOf<string>();
});

test("Taggable's addTag returns the instance, so chains keep their type", () => {
  const instance = new (Taggable(Entity))("e-1").addTag("a");
  expectTypeOf(instance.tags).toEqualTypeOf<string[]>();
  expectTypeOf(instance.hasTag("a")).toEqualTypeOf<boolean>();
  expectTypeOf(instance.id).toEqualTypeOf<string>();
});

test("two mixins compose into one type", () => {
  const instance = new (Taggable(Timestamped(Entity)))("e-1");
  expectTypeOf(instance.createdAt).toEqualTypeOf<number>();
  expectTypeOf(instance.tags).toEqualTypeOf<string[]>();
  expectTypeOf(instance.describe()).toEqualTypeOf<string>();
});

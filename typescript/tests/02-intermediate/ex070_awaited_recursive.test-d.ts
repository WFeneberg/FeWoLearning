import { expectTypeOf, test } from "vitest";
import type { MyAwaited } from "@ex/02-intermediate/ex070_awaited_recursive/index";

test("one layer", () => {
  expectTypeOf<MyAwaited<Promise<string>>>().toEqualTypeOf<string>();
});

// The difference from ex040's Resolved, which stopped after one.
test("every layer", () => {
  expectTypeOf<MyAwaited<Promise<Promise<number>>>>().toEqualTypeOf<number>();
  expectTypeOf<MyAwaited<Promise<Promise<Promise<boolean>>>>>().toEqualTypeOf<boolean>();
});

test("a plain type is left alone", () => {
  expectTypeOf<MyAwaited<string>>().toEqualTypeOf<string>();
  expectTypeOf<MyAwaited<{ a: number }>>().toEqualTypeOf<{ a: number }>();
});

// A thenable is anything with a callable `then`, which is what await
// actually looks for.
test("a hand-written thenable is unwrapped too", () => {
  type Thenable = { then(onfulfilled: (value: string) => void): void };
  expectTypeOf<MyAwaited<Thenable>>().toEqualTypeOf<string>();
});

// T is bare in the checked position, so this distributes for free.
test("a union is unwrapped member by member", () => {
  expectTypeOf<MyAwaited<Promise<string> | number>>().toEqualTypeOf<string | number>();
});

test("null and undefined survive", () => {
  expectTypeOf<MyAwaited<null>>().toEqualTypeOf<null>();
  expectTypeOf<MyAwaited<undefined>>().toEqualTypeOf<undefined>();
});

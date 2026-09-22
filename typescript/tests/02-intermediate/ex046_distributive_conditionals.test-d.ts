import { expectTypeOf, test } from "vitest";
import type {
  ArrayMembers,
  IsStringEach,
  IsStringWhole,
} from "@ex/02-intermediate/ex046_distributive_conditionals/index";

// The two forms agree on a single type...
test("both forms agree when there is no union", () => {
  expectTypeOf<IsStringEach<string>>().toEqualTypeOf<true>();
  expectTypeOf<IsStringWhole<string>>().toEqualTypeOf<true>();
  expectTypeOf<IsStringEach<number>>().toEqualTypeOf<false>();
  expectTypeOf<IsStringWhole<number>>().toEqualTypeOf<false>();
});

// ...and disagree on a union, which is the entire row. The distributive
// form answers per member and unions the answers, giving true | false —
// which is exactly `boolean`.
test("the distributive form answers per member", () => {
  expectTypeOf<IsStringEach<string | number>>().toEqualTypeOf<boolean>();
});

test("the bracketed form answers once, about the whole union", () => {
  expectTypeOf<IsStringWhole<string | number>>().toEqualTypeOf<false>();
});

test("the bracketed form says true when every member fits", () => {
  expectTypeOf<IsStringWhole<"a" | "b">>().toEqualTypeOf<true>();
});

// The trap. `never` is the empty union, so distributing over it runs the
// conditional zero times and unions nothing at all.
test("distributing over never gives never, not the false branch", () => {
  expectTypeOf<IsStringEach<never>>().toEqualTypeOf<never>();
});

test("the bracketed form has no such problem", () => {
  expectTypeOf<IsStringWhole<never>>().toEqualTypeOf<true>();
});

test("ArrayMembers filters a mixed union", () => {
  expectTypeOf<ArrayMembers<string | number[] | boolean[]>>().toEqualTypeOf<
    number[] | boolean[]
  >();
});

test("ArrayMembers of nothing matching is never", () => {
  expectTypeOf<ArrayMembers<string | number>>().toEqualTypeOf<never>();
});

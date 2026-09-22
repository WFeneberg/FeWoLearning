import { expectTypeOf, test } from "vitest";
import type { Action } from "@ex/02-intermediate/ex054_typed_reducer/index";

// Asserted through the tag and through Extract rather than as one whole
// shape: the idiom produces intersections (`{ type: "add" } & Payload`),
// and an intersection is not the same TYPE as the flat object it
// describes, however identically the two behave.
test("Action's tag is the union of the map's keys", () => {
  expectTypeOf<Action["type"]>().toEqualTypeOf<"add" | "remove" | "clear">();
});

test("each member carries its own payload", () => {
  expectTypeOf<Extract<Action, { type: "add" }>["amount"]>().toEqualTypeOf<number>();
  expectTypeOf<Extract<Action, { type: "add" }>["item"]>().toEqualTypeOf<string>();
  expectTypeOf<Extract<Action, { type: "remove" }>["item"]>().toEqualTypeOf<string>();
});

// The payload does not leak across members: `amount` belongs to add alone.
test("a payload does not appear on the members that do not declare it", () => {
  expectTypeOf<"amount" extends keyof Extract<Action, { type: "remove" }> ? true : false>()
    .toEqualTypeOf<false>();
});

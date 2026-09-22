import { test } from "vitest";
import { assertNever } from "@ex/02-intermediate/ex055_assert_never/index";
import type { CompleteMap, Kind } from "@ex/02-intermediate/ex055_assert_never/index";

// The map's guarantee, and the reason to prefer it: the error lands at the
// DECLARATION, before any dispatch exists. While CompleteMap is still
// `unknown` it accepts any object, so this starts red.
test("an incomplete map is rejected", () => {
  // @ts-expect-error — "archived" is missing
  const _partial: CompleteMap<Kind, string> = { draft: "Draft", published: "Published" };
});

// Record's second guarantee, which a switch does not give you: a stale key
// left behind after a rename is caught too.
test("a map with a key outside the union is rejected", () => {
  const _stale: CompleteMap<Kind, string> = {
    draft: "Draft",
    published: "Published",
    archived: "Archived",
    // @ts-expect-error — "deleted" is not a Kind
    deleted: "Deleted",
  };
});

test("assertNever refuses a value that is still reachable", () => {
  const reachable = "draft" as Kind;
  // @ts-expect-error — a reachable Kind is not assignable to never
  assertNever(reachable);
});

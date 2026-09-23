import { expectTypeOf, test } from "vitest";
import type { RouteParams } from "@ex/04-expert/ex092_route_params/index";
import { matchRoute } from "@ex/04-expert/ex092_route_params/index";

// Asserted through the keys: the recursion produces an INTERSECTION of
// object types, which is not the same type as the flat object it
// describes, however identically the two behave (ex054).
test("a single parameter", () => {
  expectTypeOf<keyof RouteParams<"/users/:id">>().toEqualTypeOf<"id">();
  expectTypeOf<RouteParams<"/users/:id">["id"]>().toEqualTypeOf<string>();
});

// Two arms, because a middle parameter is followed by a slash and the
// last is not. A single-arm version finds only one of these.
test("several parameters, in the middle and at the end", () => {
  expectTypeOf<keyof RouteParams<"/users/:id/posts/:postId">>().toEqualTypeOf<
    "id" | "postId"
  >();
});

test("three, to show the recursion keeps going", () => {
  expectTypeOf<keyof RouteParams<"/a/:x/b/:y/c/:z">>().toEqualTypeOf<"x" | "y" | "z">();
});

// Paired with a parameterised pattern: `keyof unknown` is already
// `never`, so the empty case on its own is green on the untouched stub.
test("a pattern with no parameters has no keys, unlike one that has them", () => {
  expectTypeOf<
    [keyof RouteParams<"/health">, keyof RouteParams<"/users/:id">]
  >().toEqualTypeOf<[never, "id"]>();
});

test("matchRoute reports the pattern's own parameters", () => {
  const matched = matchRoute("/users/:id/posts/:postId", "/users/1/posts/2");
  expectTypeOf(matched).not.toBeUndefined;
  if (matched !== undefined) {
    expectTypeOf(matched.id).toEqualTypeOf<string>();
    expectTypeOf(matched.postId).toEqualTypeOf<string>();
  }
});

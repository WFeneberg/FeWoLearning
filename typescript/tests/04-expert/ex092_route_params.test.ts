import { describe, expect, it } from "vitest";
import { matchRoute } from "@ex/04-expert/ex092_route_params/index";

describe("ex092 matchRoute", () => {
  it("captures a single parameter", () => {
    expect(matchRoute("/users/:id", "/users/42")).toEqual({ id: "42" });
  });

  it("captures several", () => {
    expect(matchRoute("/users/:id/posts/:postId", "/users/7/posts/9")).toEqual({
      id: "7",
      postId: "9",
    });
  });

  it("matches a pattern with no parameters", () => {
    expect(matchRoute("/health", "/health")).toEqual({});
  });

  it("rejects a literal segment that differs", () => {
    expect(matchRoute("/users/:id", "/people/42")).toBeUndefined();
  });

  it("rejects a path with the wrong number of segments", () => {
    expect(matchRoute("/users/:id", "/users/42/extra")).toBeUndefined();
    expect(matchRoute("/users/:id", "/users")).toBeUndefined();
  });

  it("rejects an empty value where a parameter is expected", () => {
    expect(matchRoute("/users/:id", "/users/")).toBeUndefined();
  });
});

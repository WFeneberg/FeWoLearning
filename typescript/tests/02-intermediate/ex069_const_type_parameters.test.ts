import { describe, expect, it } from "vitest";
import { defineConfig, pathOf, route } from "@ex/02-intermediate/ex069_const_type_parameters/index";

describe("ex069 route", () => {
  it("gives the segments back", () => {
    expect(route(["users", "id"])).toEqual(["users", "id"]);
  });

  it("composes with pathOf", () => {
    expect(pathOf(route(["users", "id"]))).toBe("/users/id");
  });

  it("handles no segments", () => {
    expect(route([])).toEqual([]);
  });
});

describe("ex069 defineConfig", () => {
  it("gives the configuration back unchanged", () => {
    const config = { name: "api", tags: ["x", "y"] };
    expect(defineConfig(config)).toEqual(config);
  });
});

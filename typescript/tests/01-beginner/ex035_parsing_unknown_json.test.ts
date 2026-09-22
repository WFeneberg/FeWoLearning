import { describe, expect, it } from "vitest";
import { isConfig, parseConfig } from "@ex/01-beginner/ex035_parsing_unknown_json/index";

const VALID = '{"name":"api","port":8080,"tags":["prod","eu"]}';

describe("ex035 parseConfig", () => {
  it("accepts a well-formed config", () => {
    expect(parseConfig(VALID)).toEqual({ name: "api", port: 8080, tags: ["prod", "eu"] });
  });

  it("accepts an empty tag list", () => {
    expect(parseConfig('{"name":"api","port":1,"tags":[]}')).toEqual({
      name: "api",
      port: 1,
      tags: [],
    });
  });

  // Everything below here is what a bare `JSON.parse(text) as Config` would
  // get wrong: it passes every valid case above and none of these.
  it("rejects text that is not JSON at all", () => {
    expect(parseConfig("not json")).toBeUndefined();
  });

  it("rejects a missing field", () => {
    expect(parseConfig('{"name":"api","port":1}')).toBeUndefined();
  });

  it("rejects a field of the wrong type", () => {
    expect(parseConfig('{"name":"api","port":"8080","tags":[]}')).toBeUndefined();
  });

  it("rejects a tag list holding a non-string", () => {
    expect(parseConfig('{"name":"api","port":1,"tags":["ok",7]}')).toBeUndefined();
  });

  it("rejects valid JSON that is not an object", () => {
    expect(parseConfig("[1,2,3]")).toBeUndefined();
    expect(parseConfig("null")).toBeUndefined();
    expect(parseConfig('"a string"')).toBeUndefined();
  });
});

describe("ex035 isConfig", () => {
  it("accepts a config carrying extra fields", () => {
    expect(isConfig({ name: "api", port: 1, tags: [], extra: true })).toBe(true);
  });

  it("rejects null without throwing", () => {
    expect(isConfig(null)).toBe(false);
  });

  it("rejects an array", () => {
    expect(isConfig([])).toBe(false);
  });
});

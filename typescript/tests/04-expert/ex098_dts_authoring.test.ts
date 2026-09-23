import { describe, expect, it } from "vitest";
import {
  defaultLimit,
  formatted,
  shortSlug,
  underscored,
} from "@ex/04-expert/ex098_dts_authoring/index";

describe("ex098 shortSlug", () => {
  it("slugifies and truncates", () => {
    expect(shortSlug("Hello Brave New World", 12)).toBe("hello-brave…");
  });

  it("leaves a short title alone", () => {
    expect(shortSlug("Hello World", 40)).toBe("hello-world");
  });
});

describe("ex098 underscored", () => {
  it("passes the separator through", () => {
    expect(underscored("Hello Brave World")).toBe("hello_brave_world");
  });
});

describe("ex098 formatted", () => {
  it("uses the module's default export", () => {
    expect(formatted("Hello World")).toBe("hello-world");
  });
});

describe("ex098 defaultLimit", () => {
  it("reads the shipped default", () => {
    expect(defaultLimit()).toBe(40);
  });
});

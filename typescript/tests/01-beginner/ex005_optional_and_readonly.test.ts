import { describe, expect, it } from "vitest";
import { resolveTheme } from "@ex/01-beginner/ex005_optional_and_readonly/index";

describe("ex005 resolveTheme", () => {
  it("returns the configured theme", () => {
    expect(resolveTheme({ id: "a", theme: "dark", nickname: "ada" })).toBe("dark");
  });

  it("defaults to light when the key is absent", () => {
    expect(resolveTheme({ id: "a", nickname: undefined })).toBe("light");
  });

  it("treats an explicitly set light theme as light", () => {
    expect(resolveTheme({ id: "a", theme: "light", nickname: undefined })).toBe("light");
  });
});
